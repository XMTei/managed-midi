﻿using System;
using System.Reflection;
using System.Threading;
using NUnit.Framework;

namespace Commons.Music.Midi.Tests
{
	[TestFixture]
	public class MidiPlayerTest
	{
		[Test]
		public void PlaySimple ()
		{
			var vt = new VirtualMidiPlayerTimeManager ();
			var player = TestHelper.GetMidiPlayer (vt);
			player.PlayAsync ();
			vt.ProceedBy (200000);
			player.PauseAsync ();
			player.Dispose ();
		}

		[Ignore ("rtmidi may not be runnable depending on the test runner platform")]
		[Test]
		public void PlayRtMidi ()
		{
			var vt = new AlmostVirtualMidiPlayerTimeManager ();
			var player = TestHelper.GetMidiPlayer (vt, new RtMidi.RtMidiAccess ());
			player.PlayAsync ();
			vt.ProceedBy (200000);
			player.PauseAsync ();
			player.Dispose ();
		}

		[Ignore ("portmidi may not be runnable depending on the test runner platform")]
		[Test]
		public void PlayPortMidi ()
		{
			var vt = new AlmostVirtualMidiPlayerTimeManager ();
			var player = TestHelper.GetMidiPlayer (vt, new PortMidi.PortMidiAccess ());
			player.PlayAsync ();
			vt.ProceedBy (200000);
			player.PauseAsync ();
			player.Dispose ();
		}

		[Test]
		public void PlaybackCompletedToEnd ()
		{
			var vt = new VirtualMidiPlayerTimeManager ();
			var player = TestHelper.GetMidiPlayer (vt, null, "Commons.Music.Midi.Tests.Resources.testmidi.mid");
			bool completed = false, finished = false;
			player.PlaybackCompletedToEnd += () => completed = true;
			player.Finished += () => finished = true;
			Assert.IsTrue (!completed, "1 PlaybackCompletedToEnd already fired");
			Assert.IsTrue (!finished, "2 Finished already fired");
			player.PlayAsync ();
			vt.ProceedBy (100);
			Assert.IsTrue (!completed, "3 PlaybackCompletedToEnd already fired");
			Assert.IsTrue (!finished, "4 Finished already fired");
			vt.ProceedBy (199900);
			player.PauseAsync ();
			player.Dispose ();
			Assert.IsTrue (completed, "5 PlaybackCompletedToEnd not fired");
			Assert.IsTrue (finished, "6 Finished not fired");
		}
		
		[Test]
		public void PlaybackCompletedToEndAbort ()
		{
			var vt = new VirtualMidiPlayerTimeManager ();
			var player = TestHelper.GetMidiPlayer (vt);
			bool completed = false, finished = false;
			player.PlaybackCompletedToEnd += () => completed = true;
			player.Finished += () => finished = true;
			player.PlayAsync ();
			vt.ProceedBy (100000);
			player.PauseAsync ();
			player.Dispose (); // abort in the middle
			Assert.IsFalse( completed, "1 PlaybackCompletedToEnd fired");
			Assert.IsTrue (finished, "2 Finished not fired");
		}

		public class AlmostVirtualMidiPlayerTimeManager : VirtualMidiPlayerTimeManager
		{
			public override void WaitBy (int addedMilliseconds)
			{
				Thread.Sleep (50);
				base.WaitBy (addedMilliseconds);
			}
		}
	}
}

