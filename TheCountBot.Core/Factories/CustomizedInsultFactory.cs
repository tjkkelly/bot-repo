using System;
using System.Collections.Generic;

namespace TheCountBot.Core.Factories
{
    public class CustomizedInsultFactory
    {
        private readonly IReadOnlyList<string> _templatedInsultList;
        private readonly Random _rng;

        public CustomizedInsultFactory( IReadOnlyList<string> templatedInsultList, Random rng )
        {
            _templatedInsultList = templatedInsultList;
            _rng = rng;
        }

        public string GetInsultForUser( string username )
        {
            int _randInt = _rng.Next(0, _templatedInsultList.Count);
            return _templatedInsultList[_randInt].Replace("{username}", username);
        }
    }
}
