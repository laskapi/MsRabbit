namespace SignalrWebApp.Rabbit
{

    public static class ApplicationBuilderExtentions
    {
        //the simplest way to store a single long-living object, just for example.
        private static RabbitClient? _listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            _listener = app.ApplicationServices.GetService<RabbitClient>();
                                 
            var lifetime = app.ApplicationServices.GetService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
                     
            lifetime?.ApplicationStarted.Register(OnStarted);

            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            lifetime?.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStarted()
        {
             _listener?.startClientAsync();
        }

        private static void OnStopping()
        {
            Console.WriteLine("STOPPING  RABBIT ");

            //      _listener.Deregister();
        }
    }
}
