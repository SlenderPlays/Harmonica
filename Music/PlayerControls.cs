using Avalonia.Controls;
using Harmonica.Extensions;
using System;

namespace Harmonica.Music
{
	internal class PlayerControls
	{
		public PlayState CurrentPlayState { get; private set; } = PlayState.STOPPED;
		public RepeatState CurrentRepeatSate { get; private set; } = RepeatState.REPEAT_OFF;
		public ShuffleState CurrentShuffleState { get; private set; } = ShuffleState.SHUFFLE_OFF;

		public Button PlayPauseButton { get; private set; }
		public Button ShuffleButton { get; private set; }
		public Button RepeatButton { get; private set; }

		public const string PLAYSTATE_PAUSED_CLASS = "PlayButton";
		public const string PLAYSTATE_PLAYING_CLASS = "PauseButton";

		public const string REPEATSTATE_OFF_CLASS = "RepeatOffButton";
		public const string REPEATSTATE_ON_CLASS = "RepeatOnButton";
		public const string REPEATSTATE_ONE_CLASS = "RepeatOneButton";

		public const string SHUFFLESTATE_OFF_CLASS = "ShuffleOffButton";
		public const string SHUFFLESTATE_ON_CLASS = "ShuffleOnButton";

		public PlayerControls(Button playPauseButton, Button shuffleButton, Button repeatButton)
		{
			PlayPauseButton = playPauseButton ?? throw new ArgumentNullException(nameof(playPauseButton));
			ShuffleButton = shuffleButton ?? throw new ArgumentNullException(nameof(shuffleButton));
			RepeatButton = repeatButton ?? throw new ArgumentNullException(nameof(repeatButton));
		}

		private string StateToClass(PlayState playState)
		{
			switch (playState)
			{
				case PlayState.STOPPED:
					return PLAYSTATE_PAUSED_CLASS;
				case PlayState.PLAYING:
					return PLAYSTATE_PLAYING_CLASS;
			}
			throw new ArgumentOutOfRangeException(nameof(playState));
		}
		private string StateToClass(RepeatState repeatState)
		{
			switch (repeatState)
			{
				case RepeatState.REPEAT_OFF:
					return REPEATSTATE_OFF_CLASS;
				case RepeatState.REPEAT_ON:
					return REPEATSTATE_ON_CLASS;
				case RepeatState.REPEAT_ONE:
					return REPEATSTATE_ONE_CLASS;
			}
			throw new ArgumentOutOfRangeException(nameof(repeatState));
		}
		private string StateToClass(ShuffleState shuffleState)
		{
			switch (shuffleState)
			{
				case ShuffleState.SHUFFLE_OFF:
					return SHUFFLESTATE_OFF_CLASS;
				case ShuffleState.SHUFFLE_ON:
					return SHUFFLESTATE_ON_CLASS;
			}
			throw new ArgumentOutOfRangeException(nameof(shuffleState));
		}

		public void SetPlayState(PlayState newState)
		{
			if (newState == CurrentPlayState) return;

			PlayPauseButton.ReplaceClass(StateToClass(CurrentPlayState), StateToClass(newState));
			CurrentPlayState = newState;
		}
		public void TogglePlayState()
		{
			PlayState newState;
			if (CurrentPlayState == PlayState.STOPPED)
			{
				newState = PlayState.PLAYING;
			}
			else
			{
				newState = PlayState.STOPPED;
			}

			SetPlayState(newState);
		}

		public void SetShuffleState(ShuffleState newState)
		{
			if(newState == CurrentShuffleState) return;

			ShuffleButton.ReplaceClass(StateToClass(CurrentShuffleState), StateToClass(newState));
			CurrentShuffleState = newState;
		}
		public void ToggleShuffleState()
		{
			ShuffleState newState;
			if(CurrentShuffleState == ShuffleState.SHUFFLE_OFF)
			{
				newState = ShuffleState.SHUFFLE_ON;
			}
			else
			{
				newState = ShuffleState.SHUFFLE_OFF;
			}

			SetShuffleState(newState);
		}

		public void SetRepeatState(RepeatState newState)
		{
			if (newState == CurrentRepeatSate) return;

			RepeatButton.ReplaceClass(StateToClass(CurrentRepeatSate), StateToClass(newState));
			CurrentRepeatSate = newState;
		}
		public void ToggleRepeatState()
		{
			RepeatState newState = RepeatState.REPEAT_OFF;
			switch (CurrentRepeatSate)
			{
				case RepeatState.REPEAT_OFF:
					newState = RepeatState.REPEAT_ON;
					break;
				case RepeatState.REPEAT_ON:
					newState = RepeatState.REPEAT_ONE;
					break;
				case RepeatState.REPEAT_ONE:
					newState = RepeatState.REPEAT_OFF;
					break;
			}

			SetRepeatState(newState);
		}
	}
	internal enum PlayState
	{
		STOPPED = 0,
		PLAYING = 1
	}

	internal enum RepeatState
	{
		REPEAT_OFF = 0,
		REPEAT_ON = 1,
		REPEAT_ONE = 2
	}

	internal enum ShuffleState
	{
		SHUFFLE_OFF = 0,
		SHUFFLE_ON = 1
	}
}