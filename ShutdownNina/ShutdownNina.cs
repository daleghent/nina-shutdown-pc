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
using NINA.Sequencer.SequenceItem;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DaleGhent.NINA.ShutdownPc.ShutdownNina {

    [ExportMetadata("Name", "Shutdown N.I.N.A.")]
    [ExportMetadata("Description", "Promptly causes N.I.N.A. to exit")]
    [ExportMetadata("Icon", "PowerSVG")]
    [ExportMetadata("Category", "Utility")]
    [Export(typeof(ISequenceItem))]
    [JsonObject(MemberSerialization.OptIn)]
    [method: ImportingConstructor]
    public class ShutdownNina() : SequenceItem {
        public override Task Execute(IProgress<ApplicationStatus> progress, CancellationToken ct) {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown();
            });

            // Unreached code, but required to satisfy the interface
            return Task.CompletedTask;
        }

        private ShutdownNina(ShutdownNina copyMe) : this() {
            CopyMetaData(copyMe);
        }

        public override object Clone() {
            return new ShutdownNina(this) { };
        }

        public override string ToString() {
            return $"Category: {Category}, Item: {Name}";
        }
    }
}