using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwesomeBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _services;
        private readonly AudioService _audio;

        public Commands(CommandService services, AudioService audio)
        {
            _services = services;
            _audio = audio;
        }


        [Command("yay")]
        public async Task PingAsync(SocketGuildUser user)
        {
            await ReplyAsync(user.Mention + " è un coglione!");
        }

        [Command("poke_user")]
        public async Task PokeUser(SocketGuildUser user)
        {
            await ReplyAsync($"{ user.Mention }, sei stato pokkato!", true);
        }

        [Command("rng")]
        public async Task Rng(int min, int max)
        {
            Random rnd = new Random();
            int rng = rnd.Next(min, max);
            await ReplyAsync($"{ Context.User.Mention } => Random number: " + rng.ToString());
        }

        [Command("help")]
        public async Task Help()
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("Help")
                .WithDescription("Questi sono i comandi che puoi usare:")
                .WithColor(new Color(237, 224, 40));
            foreach (var module in _services.Modules)
            {
                string description = null;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $".!{cmd.Aliases.First()}\n";
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            await ReplyAsync("", false, builder.Build());
        }

        [Command("roles")]
        public async Task ShowRoles()
        {
            int index = 1;
            string users = string.Empty;
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle("Ruoli")
                .WithDescription("Ecco i ruoli presenti:")
                .WithColor(new Color(255, 170, 86));
            foreach (var role in Context.Guild.Roles)
            {
                if (!string.IsNullOrWhiteSpace(role.Name))
                {
                    builder.AddField(index.ToString(), $"** {role} **");
                    index++;
                }
            }
            await ReplyAsync("", false, builder.Build());
        }

        [Command("addrole")]
        public async Task AutoRole(string role)
        {
            var user = Context.User;
            if (Context.Channel.Name == "roles")
            {
                var _role = Context.Guild.Roles.FirstOrDefault(x => x.Name == role.ToUpper());
                if (Context.User.Id != 246985081246318594)
                    if (!_role.Permissions.MoveMembers && !_role.Permissions.KickMembers && !_role.Permissions.BanMembers && !_role.Permissions.MuteMembers && !_role.Permissions.DeafenMembers && !_role.Permissions.Administrator)
                        await (user as IGuildUser).AddRoleAsync(_role);
                    else
                        await ReplyAsync($"{Context.User.Mention}, non hai i permessi per farlo!", true);
                else
                    await (user as IGuildUser).AddRoleAsync(_role);
            }
        }

        [Command("botping")]
        public async Task Ping()
        {
            await ReplyAsync($"Ping: {Context.Client.Latency}");
        }

        #region music
        //[Command("connect", RunMode = RunMode.Async)]
        //public async Task ConnectToChannel()
        //{
        //    await _audio.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
        //}

        //[Command("stop")]
        //public async Task Stop()
        //{
        //    await _audio.LeaveAudio(Context.Guild);
        //}

        //[Command("play", RunMode = RunMode.Async)]
        //public async Task PlaySong([Remainder] string song)
        //{
        //    await _audio.SendAudioAsync(Context.Guild, Context.Channel, song);
        //}
        #endregion music

        [Command("spam", RunMode = RunMode.Async)]
        public async Task Spam(SocketGuildUser user, int times)
        {
            string msg = string.Empty;
            for (int i = 0; i < times; i++)
            {
                msg += user.Mention + " ";
            }
            await ReplyAsync(msg, true);
        }

        [Command("purge", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int number)
        {
            var messages = await Context.Channel.GetMessagesAsync(number + 1).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            const int delay = 5000;
            var msg = await ReplyAsync($"{number} messaggi eliminati! Questo messaggio verrà eliminato tra {delay / 1000} secondi");
            await Task.Delay(delay);
            await msg.DeleteAsync();
        }

        [Command("mute")]
        public async Task FakeMute(SocketGuildUser userMuted)
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            if (Context.Client.GetUser(userMuted.Id) != null)
            {
                await user.ModifyAsync(x => x.Mute = true);
                await ReplyAsync($"{Context.User.Mention} non si muta {userMuted.Mention}!");
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention}, l'utente non esiste!");
            }
        }

        [Command("punish")]
        public async Task PunishUser(SocketGuildUser user)
        {
            if (Context.Client.GetUser(user.Id) != null)
            {
                await user.KickAsync("Sei stato punito!");
                await ReplyAsync($"{user.Username} è stato punito!");
            }
        }
    }
}