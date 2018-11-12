using System;
using TheCountBot.Application.Models.Enums;

namespace TheCountBot.Application.Models
{
    public class BotCommand
    {

        public BotCommandEnum commandType { get; set; }


        public BotCommand( String command )
        {
            parseCommand( command );
        }

        private void parseCommand( String command )
        {
            if ( command == "/stats" || command == "/stats@the_cnt_bot" )
            {
                commandType = BotCommandEnum.limitedStats;
            }
            else if ( command == "/fullstats" || command == "/fullstats@the_cnt_bot" )
            {
                commandType = BotCommandEnum.fullStats;
            }
            else if ( command == "/mystats" || command == "/mystats@the_cnt_bot" )
            {
                commandType = BotCommandEnum.individualStats;
            }
            else
            {
                commandType = BotCommandEnum.noCommand;
            }
        }
    }
}
