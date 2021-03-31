using System;

namespace FarmerAndPartners.Helpers
{
    public class ViewModelEventArgs : EventArgs
    {
        public readonly string message;
        public readonly int oldViewModelId;

        public ViewModelEventArgs(string message, int oldViewModelId)
        {
            this.message = message;
            this.oldViewModelId = oldViewModelId;
        }
    }
}
