using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class GdUnitHealthTest
{
	[TestCase]
   public void Health() {
	 HealthBar hb = new();
	 hb.Value = 100;

	 hb.SetHealth(50);
	 
	 AssertFloat(hb.Value).IsEqual(50);
	 hb.Free();
   }
}
