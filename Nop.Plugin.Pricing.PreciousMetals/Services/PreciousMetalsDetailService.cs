/**
 * @Name PreciousMetalsDetailService.cs
 * @Purpose 
 * @Date 12 January 2021, 12:00:21
 * @Author S.Deckers
 * @Description 
 */

namespace Nop.Plugin.Pricing.PreciousMetals.Services
{
	#region -- Using directives --
	using System;
	using System.Linq;

	using Nop.Core;
	using Nop.Data;
	using Nop.Services.Logging;

	using Nop.Plugin.Pricing.PreciousMetals.Domain;

	//using d=System.Diagnostics.Debug;
	using d=Nop.Plugin.Pricing.PreciousMetals.Helpers.DiagnosticsWriter;
	#endregion	

	public interface IPreciousMetalsDetailService
	{
		void					Insert			( PreciousMetalsDetail item);		
		void					Update			( PreciousMetalsDetail item);
		void					Delete			( int productId);
		PreciousMetalsDetail	GetByProductId	( int productId);
	}

    public class PreciousMetalsDetailService : IPreciousMetalsDetailService
    {
		private readonly IRepository<PreciousMetalsDetail>	_repository;
		private readonly ILogger							_logger;

		/// <summary>
		/// Constuction
		/// </summary>
		/// <param name="repository"></param>
        public PreciousMetalsDetailService
		(
			IRepository<PreciousMetalsDetail>	repository
		,	ILogger								logger
		)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._repository	= repository;
			this._logger		= logger;
        }

		public PreciousMetalsDetail GetByProductId( int productId)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Format( "{0}", productId)));
            if( productId == 0)
			{
                return null;
			}

			PreciousMetalsDetail item = _repository.Table.Where( x => x.ProductId == productId).FirstOrDefault( );

            return( item);
		}

		public void Insert( PreciousMetalsDetail item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			_repository.Insert( item);
		}

		public void Update( PreciousMetalsDetail item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			_repository.Update( item);
		}

		public void Delete( int productId)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			 _repository.Delete( _repository.Table.Where( x => x.ProductId == productId).FirstOrDefault( ));
		}
	}
}