namespace FlazorTemplate.Models
{
    /// <summary>
    /// Represents a customer that the user has permissions to administer.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// The customer's ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The customer's Parent ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// The customer's Name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Flag if the customer is a division head
        /// </summary>
        public bool IsDivisionHead { get; set; }
    }
}
