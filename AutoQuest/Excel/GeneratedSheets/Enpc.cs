using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace AutoQuest.Excel.GeneratedSheets
{
    internal class Enpc : Lumina.Excel.GeneratedSheets2.ENpcBase
    {
        public LazyRow<Lumina.Excel.GeneratedSheets2.ENpcDressUp> ENpcDressUp;
        public LazyRow<Lumina.Excel.GeneratedSheets2.ENpcResident> ENpcResident;
        public override void PopulateData(RowParser parser, GameData gameData, Language language)
        {
            base.PopulateData(parser, gameData, language);
            ENpcDressUp = new LazyRow<Lumina.Excel.GeneratedSheets2.ENpcDressUp>(gameData, RowId, language);
            ENpcResident = new LazyRow<Lumina.Excel.GeneratedSheets2.ENpcResident> (gameData, RowId, language);
        }
        public override string ToString()
        {
            return $"Enpc#{RowId}";
        }
    }
}
