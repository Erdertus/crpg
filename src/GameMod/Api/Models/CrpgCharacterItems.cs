namespace Crpg.GameMod.Api.Models
{
    // Copy of Crpg.Application.Characters.Model.CharacterItemsViewModel
    internal class CrpgCharacterItems
    {
        public CrpgItem? HeadItem { get; set; }
        public CrpgItem? ShoulderItem { get; set; }
        public CrpgItem? BodyItem { get; set; }
        public CrpgItem? HandItem { get; set; }
        public CrpgItem? LegItem { get; set; }
        public CrpgItem? MountItem { get; set; }
        public CrpgItem? MountHarnessItem { get; set; }
        public CrpgItem? Weapon1Item { get; set; }
        public CrpgItem? Weapon2Item { get; set; }
        public CrpgItem? Weapon3Item { get; set; }
        public CrpgItem? Weapon4Item { get; set; }
        public bool AutoRepair { get; set; }
    }
}
