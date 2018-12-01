namespace TheCountBot.Core
{
    public class PreviousMessageStateTracker
    {
        private int? _lastNumber;
        private string _lastUserToSendCorrectMessage;

        public PreviousMessageStateTracker()
        {
            _lastNumber = null;
            _lastUserToSendCorrectMessage = null;
        }

        public bool IsUserNumberCompositionValid( string username, int numberToCheck )
        {
            if ( _lastUserToSendCorrectMessage == username )
            {
                return false;
            }

            if ( numberToCheck != _lastNumber + 1 )
            {
                return false;
            }

            return true;
        }

        public void SetMessageStateToInvalid()
        {
            _lastNumber = null;
            _lastUserToSendCorrectMessage = null;
        }

        public void SetMessageStateToValid( string username, int number )
        {
            _lastNumber = number;
            _lastUserToSendCorrectMessage = username;
        }
    }
}
