using AldursLab.WurmApi.Tests.TempDirs;

namespace AldursLab.WurmApi.Tests.Builders.WurmClient
{
    static class WurmClientMockBuilder
    {
        public static WurmClientMock Create()
        {
            var dir = TempDirectoriesFactory.CreateEmpty();
            return new WurmClientMock(dir, true);
        }

        public static WurmClientMock CreateWithoutBasicDirs()
        {
            var dir = TempDirectoriesFactory.CreateEmpty();
            return new WurmClientMock(dir, false);
        }
    }
}
