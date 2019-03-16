using System;

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public class Configurable<T> : IEquatable<T>, IEquatable<Configurable<T>> where T : IEquatable<T> {
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)

    private T value;

    public void Set(T value)
    {
        this.value = value;
    }

    public T Get()
    {
        return value;
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public bool Equals(T other)
    {
        return value.Equals(other);
    }

    public bool Equals(Configurable<T> other)
    {
        return value.Equals(other.Get());
    }

    public static bool operator !=(Configurable<T> a, Configurable<T> b)
    {
        return !a.Get().Equals(b.Get());
    }
    public static bool operator ==(Configurable<T> a, Configurable<T> b)
    {
        return a.Get().Equals(b.Get());
    }
    public static bool operator !=(Configurable<T> a, T b)
    {
        return !a.Get().Equals(b);
    }
    public static bool operator ==(Configurable<T> a, T b)
    {
        return a.Get().Equals(b);
    }
}