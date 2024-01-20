using Microsoft.Extensions.DependencyInjection;


namespace MobileBanking.BusinessLogic
{

    public static class ServiceLocator
    {
        public static IServiceProvider Current { get; set; }

        public static T GetInstance<T>()
        {
            return Current.GetService<T>();
        }
    }

}
