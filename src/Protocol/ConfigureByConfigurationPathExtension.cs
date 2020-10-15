#if false
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Binder;
using Microsoft.Extensions.Primitives;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration
{
    public static class ConfigureByConfigurationPathExtension
    {
        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return Configure<TOptions>(services, null);
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="sectionName">The name of the options instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string? sectionName)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(
                _ => new ConfigurationChangeTokenSource<TOptions>(
                    Options.Options.DefaultName,
                    sectionName == null ? _.GetRequiredService<IConfiguration>() : _.GetRequiredService<IConfiguration>().GetSection(sectionName)
                )
            );
            return services.AddSingleton<IConfigureOptions<TOptions>>(
                _ => new NamedConfigureFromConfigurationOptions<TOptions>(
                    Options.Options.DefaultName,
                    sectionName == null ? _.GetRequiredService<IConfiguration>() : _.GetRequiredService<IConfiguration>().GetSection(sectionName)
                )
            );
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return Configure<TOptions>(services, Options.Options.DefaultName, configureBinder);
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="sectionName">The name of the options instance.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services, string sectionName, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions();
            services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(_ => new ConfigurationChangeTokenSource<TOptions>(Options.Options.DefaultName, _.GetRequiredService<IConfiguration>().GetSection(sectionName)));
            return services.AddSingleton<IConfigureOptions<TOptions>>(_ => new NamedConfigureFromConfigurationOptions<TOptions>(Options.Options.DefaultName, _.GetRequiredService<IConfiguration>().GetSection(sectionName), configureBinder));
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="builder">The <see cref="OptionsBuilder&lt;TOptions&gt;"/> to configure.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> Configure<TOptions>(this OptionsBuilder<TOptions> builder)
            where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return Configure(builder, Options.Options.DefaultName);
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="builder">The <see cref="OptionsBuilder&lt;TOptions&gt;"/> to configure.</param>
        /// <param name="sectionName">The name of the options instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> Configure<TOptions>(this OptionsBuilder<TOptions> builder, string sectionName)
            where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            Configure<TOptions>(builder.Services, name);
            return builder;
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="builder">The <see cref="OptionsBuilder&lt;TOptions&gt;"/> to configure.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> Configure<TOptions>(this OptionsBuilder<TOptions> builder, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return Configure(builder, Options.Options.DefaultName, configureBinder);
        }

        /// <summary>
        /// Registers a injected configuration service which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="builder">The <see cref="OptionsBuilder&lt;TOptions&gt;"/> to configure.</param>
        /// <param name="sectionName">The name of the options instance.</param>
        /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static OptionsBuilder<TOptions> Configure<TOptions>(this OptionsBuilder<TOptions> builder, string sectionName, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }


            Configure<TOptions>(builder.Services, name, configureBinder);
            return builder;
        }
    }
}
#endif
