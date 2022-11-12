#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using DaleGhent.NINA.ShutdownPc.Utility;
using Newtonsoft.Json;
using NINA.Core.Model;
using NINA.Core.Utility;
using NINA.Sequencer.SequenceItem;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DaleGhent.NINA.ShutdownPc {

    [ExportMetadata("Name", "Shutdown PC")]
    [ExportMetadata("Description", "Shuts down the computer")]
    [ExportMetadata("Icon", "PowerSVG")]
    [ExportMetadata("Category", "Utility")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    public class ShutdownPcInstruction : SequenceItem {
        private readonly string shutdownExe = @"C:\Windows\System32\shutdown.exe";
        private int shutdownMode = 0;

        [ImportingConstructor]
        public ShutdownPcInstruction() {
        }

        [JsonProperty]
        public int ShutdownMode {
            get => shutdownMode;
            set {
                shutdownMode = value;
                RaiseAllPropertiesChanged();
            }
        }

        public IList<string> ShutdownModes => ItemLists.ShutdownModes;
        public string ShutdownModeText => ItemLists.ShutdownModes[shutdownMode];

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken ct) {
            RunShutdown();

            return Task.CompletedTask;
        }

        private void RunShutdown() {
            List<string> args = new();

            switch (ShutdownMode) {
                case 1:
                    args.Add("/h");
                    break;

                default:
                    args.Add("/s");
                    args.Add("/t 3");
                    args.Add("/d p:0:0");
                    args.Add("/hybrid");
                    break;
            }

            args.Add("/f");

            var shutdownCmd = new ProcessStartInfo(shutdownExe) {
                Arguments = string.Join(" ", args.ToArray()),
                UseShellExecute = true,
            };

            Logger.Info($"Shutting down the computer with: {shutdownCmd.FileName} {shutdownCmd.Arguments}");

            try {
                var cmd = Process.Start(shutdownCmd);
                Logger.Info($"{shutdownExe} started with PID {cmd.Id}");
            } catch (Exception ex) {
                throw new SequenceEntityFailedException(ex.Message);
            }

            return;
        }

        private ShutdownPcInstruction(ShutdownPcInstruction copyMe) : this() {
            CopyMetaData(copyMe);
        }

        public override object Clone() {
            return new ShutdownPcInstruction(this) {
                ShutdownMode = ShutdownMode,
            };
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(ShutdownPcInstruction)}";
        }
    }
}