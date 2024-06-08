using GdUnit4;
using static GdUnit4.Assertions;

[TestSuite]
public class GdUnitAmmoTest
{
   [TestCase]
   public void Ammo() {
	 AmmoBar ab = new();
	 ab.Value = 0.5;

	 ab.SetAmmo(1);
	 
	AssertFloat(ab.Value).IsEqual(1);
    ab.Free();
   }

}
