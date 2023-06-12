using Hypertonic.GridPlacement.Enums;

namespace Hypertonic.GridPlacement
{
    public class PlacementValidResponse
    {
        public bool Valid { get; set; }
        public PlacementInvalidReason? PlacementInvalidReason { get; set; }

        public PlacementValidResponse(bool valid, PlacementInvalidReason? placementInvalidReason)
        {
            Valid = valid;
            PlacementInvalidReason = placementInvalidReason;
        }
    }
}