using System.Collections.Generic;
using System.Text;

namespace MobileBanking.BusinessLogic
{
    public class ResponseBO
    {
        public enum ResponseStatus { Success, Error, Exception }

        public ResponseStatus Status { get; set; }
        public List<string> Messages { get; set; }

        public ResponseBO()
        {
            Status= ResponseStatus.Success;
            Messages = new List<string>();
        }

        public void AddSuccess(string message)
        {
            Messages.Add(message);
        }

        public void AddError(string message)
        {
            if (Status < ResponseStatus.Error)
                Status = ResponseStatus.Error;
            Messages.Add(message);
        }

        public void AddException(string message)
        {
            if (Status < ResponseStatus.Exception)
                Status = ResponseStatus.Exception;
            Messages.Add(message);
        }

        public void ImportMessages(IEnumerable<string> messages, ResponseStatus status)
        {
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }

                // Update status if the imported response has a 'higher' status
                if (status > Status)
                   Status = status;
                
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var m in Messages)
            {
                sb.AppendLine(m);
            }
            return Status.ToString() + ": " + sb.ToString();
        }

    }


    public class ResponseBO<T> : ResponseBO
    {      
        public T Data { get; set; }

    }
}
