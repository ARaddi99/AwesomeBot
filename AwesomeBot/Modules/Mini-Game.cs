using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeBot.Modules
{
    //Il mini-gioco dovrà essere composto in 2 fasi:
    //1- I due utenti coinvolti nella sfida decidono il numero che uscirà
    //2- Il bot rollerà un numero causale da 1 a X, l'utente che ha scelto il numero più vicino a quello uscito, vincerà.

    public class Mini_Game : ModuleBase<SocketCommandContext>
    {
        public static bool locked = false;
        public static SocketGuildUser challenger;
        public static SocketGuildUser opponent;
        public static bool declined = false;
        public static int challengerNumber = -1;
        public static int opponentNumber = -1;
        Random random = new Random();
        public static int opponentDifference = 0, challengerDifference = 0, pointsWon = 0;
        public static int rnd;

        [Command("challenge", RunMode = RunMode.Async)]
        public async Task Challenge(SocketGuildUser user)
        {
            if (Context.Channel.Name == "discordbetaenvironment")
            {
                if ((SocketGuildUser)Context.User != user)
                {
                    await ReplyAsync($"{user.Mention}, {Context.User.Mention} ti ha sfidato. Vuoi accettare? .!y .!n");
                    challenger = (SocketGuildUser)Context.User;
                    opponent = user;
                }
                else
                {
                    await ReplyAsync($"{Context.User.Mention}, non puoi sfidare te stesso! :facepalm:");
                }
            }
        }

        [Command("y", RunMode = RunMode.Async)]
        public async Task Accepted()
        {
            if (Context.Channel.Name == "discordbetaenvironment")
            {
                if ((SocketGuildUser)Context.User != challenger & (SocketGuildUser)Context.User == opponent & challenger != null)
                {
                    if (!locked && !declined)
                    {
                        await ReplyAsync($"{challenger.Mention}, {Context.User.Username} ha accettato la tua sfida!");
                        declined = false;
                        locked = true;
                        await ReplyAsync($"{challenger.Mention}, {Context.User.Mention} adesso usate il comando .!choose per scegliere il vostro numero (0 - 100)");
                    }
                    else if (declined)
                    {
                        await ReplyAsync($"{Context.User.Mention}, prima hai rifiutato la sfida.. :facepalm:");
                    }
                    else
                    {
                        await ReplyAsync("La sfida è già stata accettata!");
                    }
                }
                else if (challenger == null)
                {
                    await ReplyAsync($"{Context.User.Mention}, nessuno ti ha sfidato.. Che tristezza.. :facepalm:");
                }
                else
                {
                    await ReplyAsync($"{Context.User.Username}, {challenger.Mention} non ha sfidato te!");
                    locked = false;
                }
            }
        }

        [Command("n", RunMode = RunMode.Async)]
        public async Task Declined()
        {
            if (Context.Channel.Name == "discordbetaenvironment")
            {
                if ((SocketGuildUser)Context.User != challenger & (SocketGuildUser)Context.User == opponent)
                {
                    await ReplyAsync($"{challenger.Mention}, {Context.User.Mention} ha rifiutato la tua sfida!");
                    declined = true;
                }
                else if ((SocketGuildUser)Context.User != opponent)
                {
                    await ReplyAsync($"{Context.User.Username}, {challenger.Mention} non ha sfidato te!");
                }
            }
        }

        [Command("choose", RunMode = RunMode.Async)]
        public async Task ChooseNumber(int number)
        {
            if (Context.Channel.Name == "discordbetaenvironment")
            {
                rnd = random.Next(0, 100);
                Player playerChallenger = new Player(challenger);
                Player playerOpponent = new Player(opponent);
                if ((SocketGuildUser)Context.User == challenger)
                {
                    await ReplyAsync($"{Context.User.Mention} ha scelto il numero: {number}");
                    challengerNumber = number;
                    if (opponentNumber != -1)
                    {
                        await ReplyAsync($"È uscito il numero: {rnd}");
                        Calcola();
                        await ReplyAsync($"Il vincitore è: {Winner().Mention} e ha vinto: {pointsWon} punti! :smile:");
                        if (Winner().Equals(challenger))
                        {
                            playerChallenger.AddPoints(pointsWon);
                            await ReplyAsync($"{playerChallenger.User.Username} ha già: {playerChallenger.CurrentPoints} punti");
                        }
                        else
                        {
                            playerOpponent.AddPoints(pointsWon);
                            await ReplyAsync($"{playerOpponent.User.Username} ha già: {playerOpponent.CurrentPoints} punti");
                        }
                        locked = false;
                    }
                }

                else if ((SocketGuildUser)Context.User == opponent)
                {
                    await ReplyAsync($"{Context.User.Mention} ha scelto il numero: {number}");
                    opponentNumber = number;
                    if (challengerNumber != -1)
                    {
                        await ReplyAsync($"È uscito il numero: {rnd}");
                        Calcola();
                        await ReplyAsync($"Il vincitore è: {Winner().Mention} e ha vinto: {pointsWon} punti! :smile:");
                        if (Winner().Equals(challenger))
                        {
                            playerChallenger.AddPoints(pointsWon);
                            await ReplyAsync($"{playerChallenger.User.Username} ha già: {playerChallenger.CurrentPoints} punti");
                        }
                        else
                        {
                            playerOpponent.AddPoints(pointsWon);
                            await ReplyAsync($"{playerOpponent.User.Username} ha già: {playerOpponent.CurrentPoints} punti");
                        }
                        locked = false;
                    }
                }
                Data.PlayersList.Add(playerChallenger);
                Data.PlayersList.Add(playerOpponent);
                ChallengedEnded();
            }
        }

        [Command("points")]
        public async Task CurrentPoints()
        {
            foreach (var item in Data.PlayersList)
            {
                if (item.User.Username.Equals(Context.User.Username))
                    await ReplyAsync($"Hai: {item.CurrentPoints} punti");
            }
        }

        private void Calcola()
        {
            if (opponentNumber > rnd)
                opponentDifference = opponentNumber - rnd;
            else
                opponentDifference = rnd - opponentNumber;
            if (challengerNumber > rnd)
                challengerDifference = challengerNumber - rnd;
            else
                challengerDifference = rnd - challengerNumber;
            pointsWon = opponentDifference + challengerDifference;
        }

        public SocketGuildUser Winner()
        {
            if (opponentDifference < challengerDifference)
                return opponent;
            return challenger;
        }

        public void ChallengedEnded()
        {
            challenger = null;
            opponent = null;
        }

        private void CancelChallenge()
        {

        }
    }
}