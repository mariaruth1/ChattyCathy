 using System.Reflection;
 using Fleck;
 using helloworld.core;
 using helloworld.Infrastructure;
 using helloworld.Services;
 using lib;

 var builder = WebApplication.CreateBuilder(args);
 
 // Use PostgreSQL with Dapper for working with data
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var  Uri = new Uri(connectionString);
var usethis = string.Format(
         "Server={0};Database={1};User Id={2};Password={3};Port={4};Pooling=true;MaxPoolSize=3",
         Uri.Host,
         Uri.AbsolutePath.Trim('/'),
         Uri.UserInfo.Split(':')[0],
         Uri.UserInfo.Split(':')[1],
         Uri.Port > 0 ? Uri.Port : 5432);

 builder.Services.AddNpgsqlDataSource(usethis);

 builder.Services.AddScoped<MessageRepository>();
 builder.Services.AddScoped<MessageService>();

 var eventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);

 var app = builder.Build();

 var server = new WebSocketServer("ws://0.0.0.0:8181");
 
 //var clients = new List<IWebSocketConnection>();
 
 server.Start(ws =>
 {
     ws.OnMessage = async message =>
     {
         try
         {
             await app.InvokeClientEventHandler(eventHandlers, ws, message, ServiceLifetime.Scoped);
         }
         catch (Exception e)
         {
             Console.WriteLine(e.Message);
             Console.WriteLine(e.StackTrace);
             Console.WriteLine(e.InnerException?.Message); 
         }
     };
     ws.OnError = e =>
     {
         Console.WriteLine(e.Message);
         Console.WriteLine(e.StackTrace);
         Console.WriteLine(e.InnerException?.Message); 
     };
     
     ws.OnOpen = () =>
     {
         Console.WriteLine("Open!");
         StateService.AddConnections(ws);
     };
     
     ws.OnClose = () =>
     {
         Console.WriteLine("Close!");
         
     };
 });
 Console.ReadLine();


