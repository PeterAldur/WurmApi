namespace AldursLab.WurmApi
{
    public interface IWurmPaths
    {
        string ConfigsDirFullPath { get; }
        string CharactersDirFullPath { get; }

        string GetSkillDumpsFullPathForCharacter(CharacterName characterName);
    }
}