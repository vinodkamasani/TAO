namespace TAO.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get;protected set;  }
        public DateTime CreatedOn { get; protected set; }
        public DateTime? ModifiedOn { get; protected set; }

        protected Entity() {
        Id = Guid.CreateVersion7();
        }
    }
}
