using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp.Extensions.JsonRpc.Generators.Cache
{
    class CacheKeyHasher : IDisposable
    {
        public static bool Cache = true;
        private readonly SHA1 _hasher;

        public CacheKeyHasher()
        {
            _hasher = SHA1.Create();
        }

        public void Append(string textToHash)
        {
            var inputBuffer = Encoding.UTF8.GetBytes(textToHash);
            _hasher.TransformBlock(inputBuffer, 0, inputBuffer.Length, inputBuffer, 0);
        }

        public void Append(TypeSyntax? typeSyntax)
        {
            if (typeSyntax?.GetSyntaxName() is { } a)
            {
                Append(a);
            }
        }

        public void Append(TypeParameterListSyntax? typeParameterListSyntax)
        {
            if (typeParameterListSyntax is null or { Parameters: { Count: 0 } }) return;
            foreach (var item in typeParameterListSyntax.Parameters)
            {
                Append(item.Identifier.Text);
                Append(item.AttributeLists);
            }
        }

        public void Append(BaseListSyntax? baseListSyntax)
        {
            if (baseListSyntax is null) return;
            foreach (var item in baseListSyntax.Types)
            {
                Append(item.Type);
            }
        }

        public void Append(SyntaxList<AttributeListSyntax> attributeList)
        {
            foreach (var item in attributeList)
            {
                Append(item);
            }
        }

        public void Append(IEnumerable<PropertyDeclarationSyntax> items)
        {
            foreach (var item in items.OrderBy(z => z.Identifier.Text))
            {
                Append(item.AttributeLists);
                Append(item.Identifier.Text);
                Append(item.Type);
            }
        }

        public void Append(AttributeListSyntax attributeList)
        {
            if (attributeList is { Attributes: { Count: 0 } }) return;
            foreach (var item in attributeList.Attributes.OrderBy(z => z.Name.GetSyntaxName()))
            {
                Append(item);
            }
        }

        public void Append(AttributeSyntax attribute)
        {
            Append(attribute.Name.GetSyntaxName() ?? string.Empty);
            if (attribute.ArgumentList?.Arguments is { Count: > 0 } arguments)
            {
                foreach (var item in arguments)
                {
                    if (item.NameEquals is { })
                    {
                        Append(item.NameEquals.Name.GetSyntaxName() ?? string.Empty);
                    }

                    Append(
                        item switch {
                            { Expression: TypeOfExpressionSyntax tyof } => tyof.Type.GetSyntaxName() is { Length: > 0 } name ? name : string.Empty,
                            { Expression: LiteralExpressionSyntax { } literal } => literal.Token.Text,
                            _ => string.Empty
                        }
                    );
                }
            }
        }

        private string ConvertByteArrayToString()
        {
            _hasher.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            var sb = new StringBuilder();
            foreach (var b in _hasher.Hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public override string ToString() => ConvertByteArrayToString();

        public static implicit operator string(CacheKeyHasher value) => value.ToString();

        public void Dispose() => _hasher.Dispose();
    }
}
