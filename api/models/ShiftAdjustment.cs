using System;

namespace CAS.API.models
{
    public sealed class ShiftAdjustment
    {
        public bool Equals(ShiftAdjustment other)
        {
            return SheriffId.Equals(other.SheriffId) && Date.Equals(other.Date) && Timezone == other.Timezone;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ShiftAdjustment) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SheriffId, Date, Timezone);
        }

        public Guid SheriffId { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Timezone { get; set; }
    }
}
