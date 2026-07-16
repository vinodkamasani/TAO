namespace TAO.SharedKernel.Interfaces
{
    public interface ITenantContext
    {
        Guid? TenantId { get; }
    }
}
