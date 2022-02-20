function forEachChunk(chunks, callback, chunkSize = 50) {
    var mappedFiles = [];
    var files = chunks.concat();
    while (files.length > 0) {
        var chunk = files.splice(0, chunkSize);
        mappedFiles = mappedFiles.concat(callback(chunk));
    }
    return mappedFiles;
}

function cleanupcode(filenames) {
    var sln = require('./.nuke/parameters.json').Solution;
    return forEachChunk(filenames, chunk => [
        `dotnet jb cleanupcode ${sln} "--profile=Full Cleanup" "--disable-settings-layers=GlobalAll;GlobalPerProduct;SolutionPersonal;ProjectPersonal" "--include=${chunk.join(
            ';'
        )}"`,
    ]);
}

module.exports = {
    '*.cs': filenames => {
        return [`echo "'${filenames.join(`' '`)}'" | dotnet format --include -`].concat(cleanupcode(filenames));
    },
    '*.{csproj,targets,props,xml}': filenames =>
        forEachChunk(filenames, chunk => [`prettier --write '${chunk.join(`' '`)}'`]),
    '*.{js,ts,jsx,tsx,json,yml,yaml}': filenames =>
        forEachChunk(filenames, chunk => [`prettier --write '${chunk.join(`' '`)}'`]),
};
