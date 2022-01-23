#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

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

        [ImportingConstructor]
        public ShutdownPcInstruction() {
        }

        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken ct) {
            RunShutdown();

            return Task.CompletedTask;
        }

        private void RunShutdown() {
            List<string> args = new List<string> {
                "/s",
                "/t 3",
                "/f",
                "/d p:0:0",
                "/hybrid",
            };

            var shutdownCmd = new ProcessStartInfo(shutdownExe) {
                Arguments = string.Join(" ", args.ToArray()),
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
            return new ShutdownPcInstruction(this);
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {nameof(ShutdownPcInstruction)}";
        }
    }
}