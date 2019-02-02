
namespace StateMigrationTool
{
    internal class UIMessages
    {
        private string Message { get; set; }

        public string Show(int id)
        {
            switch(id)
            {
                case 1: Message = "Starting Operation";
                    break;

                case 2:
                    Message = "Operation Completed";
                    break;

                case 3:
                    Message = "0%";
                    break;

                case 4:
                    Message = "All operations completed successfully";
                    break;

                case 5:
                    Message = "Please make sure to close all apps before running this utility";
                    break;

                case 6:
                    Message = "Operation aborted due to Exception.";
                    break;

                case 7:
                    Message = "All Operations Completed Successfully";
                    break;

            }

            return Message;
        }

        public string Show(int id, string message)
        {
            switch (id)
            {
                case 1:
                    Message = $"Starting Operation on {message}";
                    break;

                case 2:
                    Message = $"Operation Completed on {message}";
                    break;

                case 3:
                    Message = $"{message}%";
                    break;
            }
            return Message;
        }
    }
}
