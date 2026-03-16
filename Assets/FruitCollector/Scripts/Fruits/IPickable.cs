public interface IPickable
{
    string Id { get; }
    string DisplayName { get; }

    // Added MaxStack here
    int MaxStack {  get; }
    void Pick(IStorable receiver);
}