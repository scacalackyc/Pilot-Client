﻿/*
 * xPilot: X-Plane pilot client for VATSIM
 * Copyright (C) 2019-2020 Justin Shannon
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see http://www.gnu.org/licenses/.
*/
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XPilot.PilotClient.Core.Events;

namespace XPilot.PilotClient
{
    public class TextCommandLine : TextBox
    {
        public event EventHandler<ClientEventArgs<string>> TextCommandReceived;
        private List<string> mTextHistory = new List<string>();
        private int mHistoryIndex = -1;
        private string mCommandLineValue = "";

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(Text))
                    {
                        RecordHistory(Text);
                        Text = "";
                        mHistoryIndex = -1;
                    }
                    return;
                case Keys.Escape:
                    if (Text.Length > 0)
                    {
                        Text = "";
                        e.Handled = true;
                    }
                    return;
                case Keys.Up:
                    Text = ScrollUp(Text);
                    SelectionStart = Text.Length;
                    e.Handled = true;
                    return;
                case Keys.Down:
                    Text = ScrollDown(Text);
                    SelectionStart = Text.Length;
                    e.Handled = true;
                    return;
            }
            mHistoryIndex = -1;
            base.OnKeyDown(e);
        }

        private string ScrollDown(string text)
        {
            if (mHistoryIndex == -1)
            {
                mCommandLineValue = text;
            }
            mHistoryIndex--;
            if (mHistoryIndex < 0)
            {
                mHistoryIndex = -1;
                return mCommandLineValue;
            }
            return mTextHistory[mHistoryIndex];
        }

        private string ScrollUp(string text)
        {
            if (mHistoryIndex == -1)
            {
                mCommandLineValue = text;
            }
            mHistoryIndex++;
            if (mHistoryIndex >= mTextHistory.Count)
            {
                mHistoryIndex = -1;
                return mCommandLineValue;
            }
            return mTextHistory[mHistoryIndex];
        }

        private void RecordHistory(string text)
        {
            mTextHistory.Insert(0, text);
            TextCommandReceived?.Invoke(this, new ClientEventArgs<string>(text));
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\u001b')
            {
                e.Handled = true;
            }
            base.OnKeyPress(e);
        }
    }
}
