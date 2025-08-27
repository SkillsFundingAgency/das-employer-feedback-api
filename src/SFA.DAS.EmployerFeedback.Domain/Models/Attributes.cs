namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class Attributes
    {
        public long AttributeId { get; set; }
        public string AttributeName { get; set; }
        public static implicit operator Attributes(Entities.Attributes source)
        {
            if (source == null)
            {
                return null;
            }

            return new Attributes
            {
                AttributeId = source.AttributeId,
                AttributeName = source.AttributeName
            };
        }
    }
}
