// using ClubService.Infrastructure.Api;
// using Microsoft.Extensions.DependencyInjection;
// using StackExchange.Redis;
//
// namespace ClubService.Infrastructure;
//
// public class RedisEventReader : IEventReader
// {
//     private readonly CancellationToken _cancellationToken;
//     private const string StreamName = "club_service_events.public.DomainEvent";
//     private IDatabase _db;
//
//     public RedisEventReader()
//     {
//         _cancellationToken = new CancellationTokenSource().Token;
//         var muxer = ConnectionMultiplexer.Connect("localhost");
//         _db = muxer.GetDatabase();
//         
//     }
//
//     public async Task ExecuteAsync()
//     {
//         if (!(await _db.KeyExistsAsync(StreamName)) ||
//             (await _db.StreamGroupInfoAsync(StreamName)).All(x=>x.Name!=GroupName))
//         {
//             await _db.StreamCreateConsumerGroupAsync(StreamName, GroupName, "0-0", true);
//         }
//         
//         var id = string.Empty;
//         while (!_cancellationToken.IsCancellationRequested)
//         {
//             if (!string.IsNullOrEmpty(id))
//             {
//                 await _db.StreamAcknowledgeAsync(StreamName, GroupName, id);
//                 id = string.Empty;
//             }
//             var result = await _db.StreamReadGroupAsync(StreamName, GroupName, "pos-member", ">", 1);
//             if (result.Any())
//             {
//                 var streamEntry = result.First();
//                 id = streamEntry.Id;
//                 var parsedEvent = ParseEvent(streamEntry);
//                 if (parsedEvent == null)
//                     continue;
//                 await memberRepository.UpdateEntityAsync(parsedEvent);
//             }
//             await Task.Delay(1000);
//         }
//     }
//
//
// }