/**
 * @Name DependencyRegistrar.cs
 * @Purpose 
 * @Date 12 January 2021, 07:46:10
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Infrastructure
{
	#region -- Using directives --
	using Autofac;

	using Nop.Core;
	using Nop.Core.Configuration;
	using Nop.Core.Infrastructure;
	using Nop.Core.Infrastructure.DependencyManagement;

	using Nop.Plugin.Pricing.PreciousMetals.Services;
	
	using Nop.Plugin.Pricing.PreciousMetals;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion

	public class DependencyRegistrar : IDependencyRegistrar
	{
		public virtual void Register( ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			builder.RegisterType<PreciousMetalsPriceCalculationService>	( ).As<Nop.Services.Catalog.IPriceCalculationService>							().InstancePerLifetimeScope();
			builder.RegisterType<PreciousMetalsDetailService>			( ).As<Nop.Plugin.Pricing.PreciousMetals.Services.IPreciousMetalsDetailService>	().InstancePerLifetimeScope();
			builder.RegisterType<PreciousMetalsQuoteService>			( ).As<Nop.Plugin.Pricing.PreciousMetals.Services.IPreciousMetalsQuoteService>	().InstancePerLifetimeScope();		
		}

		/// <summary>
		/// Order of this dependency registrar implementation
		/// </summary>
		public int Order => 1;
	}
}
