using AwesomeBot.Modules;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeBot
{
    class Program
    {
        public static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private AudioService _audio;

        public async Task Start()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _audio = new AudioService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_audio)
                .BuildServiceProvider();
            string token = "MzgzNjgwNTk2NzQxODQ5MDk5.DPn5ig.Ywr6n8JaBmgIhsMqXXIL6A9GmiA";
            _client.Log += Client_Log;
            _client.UserJoined += Client_UserJoined;
            await RegisterCommandsAsync();
            await _client.LoginAsync(Discord.TokenType.Bot, token);
            await _client.StartAsync();
            await _client.SetGameAsync(".!help ~ Fuckers");
            await Task.Delay(-1);
        }

        private async Task Client_UserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.DefaultChannel;
            await channel.SendMessageAsync($"Benvenuto, {user.Mention}!");
        }

        private Task Client_Log(Discord.LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += Client_MessageReceived;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task Client_MessageReceived(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message is null || message.Author.IsBot) return;
            int argPos = 0;
            if(message.HasStringPrefix(".!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}
