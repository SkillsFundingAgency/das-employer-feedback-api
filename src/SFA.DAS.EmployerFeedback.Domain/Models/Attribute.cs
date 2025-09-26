namespace SFA.DAS.EmployerFeedback.Domain.Models
{
    public class Attribute
    {
        public long AttributeId { get; set; }
        public string AttributeName { get; set; }
        public static implicit operator Attribute(Entities.Attribute source)
        {
            if (source == null)
            {
                return null;
            }

            return new Attribute
            {
                AttributeId = source.AttributeId,
                AttributeName = source.AttributeName
            };
        }
    }
}
