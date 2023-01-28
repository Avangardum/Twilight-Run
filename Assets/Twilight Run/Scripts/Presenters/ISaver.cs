using System;

namespace Avangardum.TwilightRun.Presenters
{
    public interface ISaver
    {
        event EventHandler<HighScoreChangedEventArgs> HighScoreChanged;
        int HighScore { get; }
    }
}