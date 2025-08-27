using PACG.Gameplay;

namespace PACG.SharedAPI
{
    public static class LocationViewModelFactory
    {
        public static LocationViewModel CreateFrom(Location location)
        {
            if (location == null) return null;

            return new LocationViewModel(location);
        }
    }
}
