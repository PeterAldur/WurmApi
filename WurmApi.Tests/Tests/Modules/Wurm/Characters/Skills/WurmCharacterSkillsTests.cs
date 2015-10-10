using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AldursLab.WurmApi.Tests.Helpers;
using NUnit.Framework;

namespace AldursLab.WurmApi.Tests.Tests.Modules.Wurm.Characters.Skills
{
    class WurmCharacterSkillsTests : WurmTests
    {
        StubbableTime.StubScope timeScope;

        public IWurmCharacterSkills TestguySkills
        {
            get { return Fixture.WurmApiManager.Characters.Get("Testguy").Skills; }
        }

        public IWurmCharacterSkills TestguytwoSkills
        {
            get { return Fixture.WurmApiManager.Characters.Get("Testguytwo").Skills; }
        }

        [SetUp]
        public void Setup()
        {
            ClientMock.PopulateFromZip(Path.Combine(TestPaksZippedDirFullPath, "WurmCharacterSkillsTests-wurmdir.7z"));

            if (timeScope != null)
            {
                timeScope.Dispose();
                timeScope = null;
            }
            timeScope = TimeStub.CreateStubbedScope();
            timeScope.OverrideNow(new DateTime(2012, 09, 23, 23, 37, 13));
        }

        [TearDown]
        public void Teardown()
        {
            timeScope.Dispose();
            timeScope = null;
        }

        [Test]
        public async Task GetsCurrentSkills()
        {
            var skill = await TestguySkills.TryGetCurrentSkillLevelAsync("Masonry",
                ServerGroupId.Freedom,
                TimeSpan.FromDays(1000));
            Expect(skill, !Null);
            Expect(skill.Value, EqualTo(58.751f));
        }

        [Test]
        public async Task FallsBackToDumpsWhenNoSkillLogs()
        {
            var skill = await TestguytwoSkills.TryGetCurrentSkillLevelAsync("Masonry",
                ServerGroupId.Freedom,
                TimeSpan.FromDays(1000));
            Expect(skill, EqualTo(73.73132f));
        }

        [Test]
        public async Task ReactsToLiveEvents()
        {
            timeScope.AdvanceTime(1);

            var player = ClientMock.AddPlayer("Newguy");
            player.Logs.WriteEventLog("42 other players are online. You are on Exodus (765 totally in Wurm).");

            var skillApi = Fixture.WurmApiManager.Characters.Get("Newguy")
                                  .Skills;

            // allow WurmApi to pick everything
            await Task.Delay(1000);

            timeScope.AdvanceTime(1);

            var awaiter = new EventAwaiter<SkillsChangedEventArgs>();
            skillApi.SkillsChanged += awaiter.GetEventHandler();

            player.Logs.WriteSkillLog("Masonry", 58.754f);
            awaiter.WaitInvocations(1);
            awaiter.WaitUntilMatch(list => list.Any(args => args.HasSkillChanged("Masonry")));

            var skill = await skillApi.TryGetCurrentSkillLevelAsync("Masonry", ServerGroupId.Freedom, TimeSpan.MaxValue);
            Expect(skill, EqualTo(58.754f));
        }
    }
}
