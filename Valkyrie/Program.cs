// entry point

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Valkyrie.Core;

using var engine = ValkyrieHostBuilder.BuildValkyrie();

ValkyrieServiceProvider.ServiceProvider = engine.Services;

{
    using var scope = ValkyrieServiceProvider.ServiceProvider.CreateScope();
    
    await engine.RunAsync().ConfigureAwait(false);
}