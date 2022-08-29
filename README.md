# Realms of Reminiscence Discord Bot (WIP)
A Discord bot written in C# to support a roleplaying Minecraft server called Realms of Reminiscence.

## Features
WIP
- Create user profiles
- Create entries of your minecraft buildings
- Start expeditions and have a chance to explore other continents

## Setup
The only required software to run this project is [docker](https://www.docker.com/).

Steps to run the project:
- Create an environment variable called `DISCORD_TOKEN` with your [discord bot token](https://github.com/reactiflux/discord-irc/wiki/Creating-a-discord-bot-&-getting-a-token) as value.
- Start a command prompt and go to the directory of the `docker-compose.yml` file.
- Run `docker compose build`. This will build the C# bot project into a docker image.
- Run `docker compose up` (or `docker compose up -d` to run in detached mode)
- You can view and edit the database by opening up `mongo-express` via http://localhost:8080/

To stop the bot, run `docker compose down` from the same command prompt.