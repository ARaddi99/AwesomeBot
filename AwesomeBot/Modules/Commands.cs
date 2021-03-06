﻿using Discord;
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
        //private readonly AudioService _audio;

        //public Commands(CommandService services, AudioService audio)
        //{
        //    _services = services;
        //    _audio = audio;
        //}


        [Command("yay")]
        public async Task PingAsync(SocketGuildUser user)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                await ReplyAsync(user.Mention + " è un coglione!");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("poke_user")]
        public async Task PokeUser(SocketGuildUser user)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                await ReplyAsync($"{ user.Mention }, sei stato pokkato!", true);
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("rng")]
        public async Task Rng(int min, int max)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                Random rnd = new Random();
                int rng = rnd.Next(min, max);
                await ReplyAsync($"{ Context.User.Mention } => Random number: " + rng.ToString());
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("help")]
        public async Task Help()
        {
            if (CanUse((SocketGuildUser)Context.User))
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
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("roles")]
        public async Task ShowRoles()
        {
            if (CanUse((SocketGuildUser)Context.User))
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
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("addrole")]
        public async Task AutoRole(string role)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                var user = Context.User;
                if (Context.Channel.Name == "roles")
                {
                    var _role = Context.Guild.Roles.FirstOrDefault(x => x.Name == role.ToUpper());
                    if (Context.User.Id != 246985081246318594)
                        if (!_role.Permissions.MoveMembers && !_role.Permissions.KickMembers && !_role.Permissions.BanMembers && !_role.Permissions.MuteMembers && !_role.Permissions.DeafenMembers && !_role.Permissions.Administrator)
                        {
                            await (user as IGuildUser).AddRoleAsync(_role);
                            await ReplyAsync($"{Context.User.Username}, adesso hai il ruolo: {_role.Name}");
                        }
                        else
                            await ReplyAsync($"{Context.User.Mention}, non hai i permessi per farlo!", true);
                    else
                    {
                        await (user as IGuildUser).AddRoleAsync(_role);
                        await ReplyAsync($"{Context.User.Username}, adesso hai il ruolo: {_role.Name}");
                    }
                }
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
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
            if (CanUse((SocketGuildUser)Context.User))
            {
                string msg = string.Empty;
                for (int i = 0; i < times; i++)
                {
                    msg += user.Mention + " ";
                }
                await ReplyAsync(msg, true);
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("purge", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int number)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                var messages = await Context.Channel.GetMessagesAsync(number + 1).Flatten();
                await Context.Channel.DeleteMessagesAsync(messages);
                const int delay = 5000;
                var msg = await ReplyAsync($"{number} messaggi eliminati! Questo messaggio verrà eliminato tra {delay / 1000} secondi");
                await Task.Delay(delay);
                await msg.DeleteAsync();
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("mute")]
        public async Task FakeMute(SocketGuildUser userMuted)
        {
            if (CanUse((SocketGuildUser)Context.User))
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
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("punish")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task PunishUser(SocketGuildUser user)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                if (Context.Client.GetUser(user.Id) != null)
                {
                    await user.KickAsync("Sei stato punito!");
                    await ReplyAsync($"{user.Username} è stato punito!");
                }
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("invite")]
        public async Task CreateInvite()
        {
            const string invite = "https://discord.gg/3NUvHMa";
            await Context.User.SendMessageAsync($"Ecco il tuo invito: \n{invite}");
        }

        [Command("fede")]
        public async Task MuteFederico()
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                var user = Context.Guild.GetUser(247041061061525504);
                var message = await Context.Channel.GetMessagesAsync(1).Flatten();
                await user.ModifyAsync(x => x.Mute = true);
                await Context.Channel.DeleteMessagesAsync(message);
                await user.SendMessageAsync($"Sei stato mutato perchè hai trivellato il cazzo a {Context.User.Username}");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("pshale")]
        public async Task KickAlessandro()
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                const int delay = 3000;
                var user = Context.Guild.GetUser(246926744907415553);
                await ReplyAsync($"{user.Mention} ha frantumato i coglioni peggio di una frantumatrice meccanica");
                await Task.Delay(delay);
                await user.KickAsync();
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("nessuno")]
        public async Task Nessuno()
        {
            await ReplyAsync("Loris non è nessuno per avere un comando personalizzato..");
        }

        [Command("ritardato")]
        public async Task Unmute()
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            if (user.IsMuted)
            {
                await user.ModifyAsync(x => x.Mute = false);
                await ReplyAsync("Va beh, per mutarti da solo effettivamente.. :facepalm:");
            }
            else
            {
                await user.ModifyAsync(x => x.Mute = true);
                await ReplyAsync("Allora farai meglio a stare zitto.. :face_palm:");
            }
        }

        [Command("norole")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task RemoveRole(string role, SocketGuildUser user)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                int counter = 0;
                foreach (var ruolo in Context.Guild.Roles)
                {
                    if (ruolo.Name.Equals(role.ToUpper()))
                        await user.RemoveRoleAsync(Context.Guild.GetRole(ruolo.Id));
                    else
                        counter++;
                }
                if (counter == Context.Guild.Roles.Count)
                    await ReplyAsync("Ruolo non trovato");
                else
                    await ReplyAsync("Ruolo rimosso");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("move", RunMode = RunMode.Async)]
        public async Task ChangeChannel(SocketGuildUser user, SocketGuildChannel channel)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                Optional<ulong> id = channel.Id;
                await user.ModifyAsync(x => x.ChannelId = id);
                await ReplyAsync("Utente spostato! :thumbsup:");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("afk", RunMode = RunMode.Async)]
        public async Task UserAFK(SocketGuildUser user)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                List<SocketVoiceChannel> list = Context.Guild.VoiceChannels.Where(x => x.Name.Equals("Afk")).ToList();
                Optional<ulong> id = list[0].Id;
                await user.ModifyAsync(x => x.ChannelId = id);
                var message = await Context.Channel.GetMessagesAsync(1).Flatten();
                await Context.Channel.DeleteMessagesAsync(message);
                await ReplyAsync($"{user} è adesso AFK");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [RequireUserPermission(GuildPermission.MoveMembers)]
        [Command("fck", RunMode = RunMode.Async)]
        public async Task BannedFromChannel(SocketGuildUser user)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                foreach (var role in Context.Guild.Roles)
                {
                    if (role.Name.Equals("FCK"))
                    {
                        await user.AddRoleAsync(role);
                        break;
                    }
                }
                await user.ModifyAsync(x => x.ChannelId = 388751938860220417);
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("removetoall", RunMode = RunMode.Async)]
        public async Task RemoveAllRole(string ruolo)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                foreach (var role in Context.Guild.Roles)
                {
                    if (role.Name.Equals(ruolo.ToUpper()))
                    {
                        foreach (var user in Context.Guild.Users)
                        {
                            await user.RemoveRoleAsync(role);
                        }
                    }
                }
                await ReplyAsync("Terminato :thumbsup:");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("givetoall", RunMode = RunMode.Async)]
        public async Task GiveAllRole(string ruolo)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                List<SocketGuildUser> users = new List<SocketGuildUser>();
                List<ulong> ids = new List<ulong>() { 246985081246318594, 246926744907415553, 246981885132013568, 247041061061525504, 246736207034318849, 262623537548886019 };
                for (int i = 0; i < ids.Count; i++)
                {
                    users.Add(Context.Guild.GetUser(ids[i]));
                }

                foreach (var role in Context.Guild.Roles)
                {
                    if (role.Name.Equals(ruolo.ToUpper()))
                    {
                        foreach (var user in users)
                        {
                            await user.AddRoleAsync(role);
                        }
                    }
                }
                await ReplyAsync("Terminato :thumbsup:");
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("lane", RunMode = RunMode.Async)]
        public async Task ChooseLane(SocketGuildUser user, SocketGuildUser user2, SocketGuildUser user3, SocketGuildUser user4, SocketGuildUser user5)
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                List<SocketGuildUser> List = new List<SocketGuildUser>() { user, user2, user3, user4, user5 };
                List<string> Lanes = new List<string>() { "Top", "Jungle", "Mid", "Adc", "Support" };
                Random random = new Random();
                int rnd;
                for (int i = 0; i < List.Count; i++)
                {
                    rnd = random.Next(0, Lanes.Count);
                    await ReplyAsync(List[i].Username + " - " + Lanes[rnd]);
                    Lanes.Remove(Lanes[rnd]);
                }
            }
            else
            {
                await ReplyAsync("Non hai i permessi per farlo");
            }
        }

        [Command("opgg", RunMode = RunMode.Async)]
        public async Task OPGG(string region, string summonerName)
        {
            await ReplyAsync("Ecco qua :smile::");
            await ReplyAsync($"http://{region}.op.gg/summoner/userName={summonerName}");
        }

        [Command("lol", RunMode = RunMode.Async)]
        public async Task MoveToLol()
        {
            if (CanUse((SocketGuildUser)Context.User))
            {
                var users = Context.Guild.Users.Where(x => x.Game.GetValueOrDefault().Name == Context.User.Game.GetValueOrDefault().Name).ToList();
                ulong id = 292773432943312897;
                foreach (var user in users)
                {
                    await user.ModifyAsync(x => x.ChannelId = id);
                }
                await ReplyAsync("Terminato :thumbsup:");
            }
        }

        [Command("test")]
        public async Task Prova()
        {
            await ReplyAsync(Context.User.Game.GetValueOrDefault().Name);
        }

        public bool CanUse(SocketGuildUser user)
        {
            var ruoli = user.Roles.Select(x => x.Name).ToList();
            foreach (var ruolo in ruoli)
            {
                if (ruolo.Equals("SUPAH"))
                    return true;
            }
            return false;
        }
    }
}