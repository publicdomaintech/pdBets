// <copyright file="MainForm.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

// Programmed by Victor L. Senior (VLS) <support@publicdomain.tech>, 2016
//
// Web: http://publicdomain.tech
//
// Sources: http://github.com/publicdomaintech/
//
// This software and associated documentation files (the "Software") is
// released under the CC0 Public Domain Dedication, version 1.0, as
// published by Creative Commons. To the extent possible under law, the
// author(s) have dedicated all copyright and related and neighboring
// rights to the Software to the public domain worldwide. The Software is
// distributed WITHOUT ANY WARRANTY.
//
// If you did not receive a copy of the CC0 Public Domain Dedication
// along with the Software, see
// <http://creativecommons.org/publicdomain/zero/1.0/>

/// <summary>
/// Main form.
/// </summary>
namespace PdBets
{
    // Directives
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// The main form of the program.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The game directories list.
        /// </summary>
        private List<string> gameDirectoriesList;

        /// <summary>
        /// The module type directories list.
        /// </summary>
        private List<string> moduleTypeDirectoriesList;

        /// <summary>
        /// The framework directories list.
        /// </summary>
        private List<string> frameworkDirectoriesList;

        /// <summary>
        /// The top level directories dictionary.
        /// </summary>
        private Dictionary<string, string> topLevelDirectoriesDictionary;

        /// <summary>
        /// The last selected tab page items dictionary.
        /// </summary>
        private Dictionary<string, string[]> lastSelectedTabPageItemsDictionary = new Dictionary<string, string[]>();

        /// <summary>
        /// The raw modules dictionary.
        /// </summary>
        private Dictionary<string, IEnumerable<IPdBets>> rawModulesDictionary = new Dictionary<string, IEnumerable<IPdBets>>();

        /// <summary>
        /// The loaded modules dictionary.
        /// </summary>
        private Dictionary<string, List<IPdBets>> loadedModulesDictionary = new Dictionary<string, List<IPdBets>>();

        /// <summary>
        /// The module types list.
        /// </summary>
        private List<string> moduleTypesList;

        /// <summary>
        /// The name of the game.
        /// </summary>
        private string gameName;

        /// <summary>
        /// The shared code.
        /// </summary>
        private SharedCode sharedCode = new SharedCode();

        /// <summary>
        /// Initializes a new instance of the <see cref="PdBets.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            /* Set game name */

            // TODO Initial game [load dynamically]
            this.gameName = "Roulette";

            /* Set directories variables */

            // Module type directories list
            this.moduleTypeDirectoriesList = new List<string>()
            {
                "Input",
                "BetSelection",
                "MoneyManagement",
                "Display",
                "Output",
                "Utilities"
            };

            // Game directories list
            this.gameDirectoriesList = new List<string>()
            {
                "Roulette",
                "Baccarat",
                "Multigame"
            };

            // Famework directories list
            this.frameworkDirectoriesList = new List<string>()
            {
                "Assemblies",
                "Configuration"
            };

            // Data directory
            string dataDirectory = Path.Combine(Application.StartupPath, "Data");

            // Top level directories dictionary
            this.topLevelDirectoriesDictionary = new Dictionary<string, string>()
            {
                { "Data", dataDirectory },
                { "Cache", Path.Combine(dataDirectory, "Cache") },
                { "Modules", Path.Combine(dataDirectory, "Modules") },
                { "License", Path.Combine(dataDirectory, "License") },
                { "Framework", Path.Combine(dataDirectory, "Framework") },
                { "Definitions", Path.Combine(dataDirectory, "Definitions") }
            };

            /* Create directories */

            // Top level
            foreach (KeyValuePair<string, string> directory in this.topLevelDirectoriesDictionary)
            {
                // Create current one
                Directory.CreateDirectory(directory.Value);
            }

            // Modules
            foreach (string moduleType in this.moduleTypeDirectoriesList)
            {
                // Set current module type directory path
                string currentModuleTypePath = Path.Combine(this.topLevelDirectoriesDictionary["Modules"], moduleType);

                // Create current module type directory
                Directory.CreateDirectory(currentModuleTypePath);

                // Create games subdirectories
                foreach (string game in this.gameDirectoriesList)
                {
                    // Create current game subdirectory
                    Directory.CreateDirectory(Path.Combine(currentModuleTypePath, game));
                }
            }

            // Framework
            foreach (string frameworkDirectory in this.frameworkDirectoriesList)
            {
                // Create current one
                Directory.CreateDirectory(Path.Combine(this.topLevelDirectoriesDictionary["Framework"], frameworkDirectory));
            }

            /* Set selected object collection dictionary */

            foreach (TabPage tabPage in this.modulesTabControl.TabPages)
            {
                // Set last selected tabpage items in dictionary
                this.lastSelectedTabPageItemsDictionary.Add(tabPage.Name, new string[0]);
            }

            /* Prepare variables to load modules */

            // Set module types
            this.moduleTypesList = this.moduleTypeDirectoriesList;

            // Remove "Utilities"
            this.moduleTypesList.Remove("Utilities");

            /* Load modules sans utilities */

            // Iterate module types
            foreach (string moduleType in this.moduleTypesList)
            {
                // Add current to raw modules dictionary
                this.rawModulesDictionary.Add(moduleType, this.LoadModules(moduleType));

                // Add blank to loaded modules dictionary
                this.loadedModulesDictionary.Add(moduleType, new List<IPdBets>());

                // Iterate modules list
                foreach (IPdBets rawModule in this.rawModulesDictionary[moduleType])
                {
                    // Add to list box
                    ((ListBox)this.Controls.Find(moduleType.ToLower() + "AvailableListBox", true)[0]).Items.Add(this.sharedCode.FileNameToDisplayName(Path.GetFileNameWithoutExtension(rawModule.GetType().Assembly.Location)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the loaded modules.
        /// </summary>
        /// <value>The loaded modules.</value>
        [ImportMany(typeof(IPdBets))]
        private IEnumerable<IPdBets> LoadedModules { get; set; }

        /// <summary>
        /// Loads the modules.
        /// </summary>
        /// <returns>The loaded modules list.</returns>
        /// <param name="moduleType">Module type.</param>
        private IEnumerable<IPdBets> LoadModules(string moduleType)
        {
            // Set directory catalog for target game
            DirectoryCatalog directoryCatalogGame = new DirectoryCatalog(Path.Combine(Path.Combine(this.topLevelDirectoriesDictionary["Modules"], moduleType), this.gameName), "*.dll");

            // Set directory catalog for multi-game
            DirectoryCatalog directoryCatalogMultigame = new DirectoryCatalog(Path.Combine(Path.Combine(this.topLevelDirectoriesDictionary["Modules"], moduleType), "Multigame"), "*.dll");

            // Aggregate both catalogs
            AggregateCatalog aggregateCatalog = new AggregateCatalog(directoryCatalogGame, directoryCatalogMultigame);

            // Set composition container
            CompositionContainer compositionContainer = new CompositionContainer(aggregateCatalog);

            // Set composition batch
            CompositionBatch compositionBatch = new CompositionBatch();

            // Add part to the composition batch
            compositionBatch.AddPart(this);

            // Execute composition
            compositionContainer.Compose(compositionBatch);

            // Return the loaded modules
            return this.LoadedModules;
        }

        /// <summary>
        /// Moves the selected list box item up.
        /// </summary>
        /// <param name="targetListBox">Target list box.</param>
        private void MoveSelectedListBoxItemUp(ListBox targetListBox)
        {
            // Check there's a selected item AND it isn't the first one
            if (targetListBox.SelectedItem != null && targetListBox.SelectedIndex > 0)
            {
                // Disable drawing
                targetListBox.BeginUpdate();

                // Insert copy above
                targetListBox.Items.Insert(targetListBox.SelectedIndex - 1, targetListBox.Text);

                // Select it
                targetListBox.SelectedIndex = targetListBox.SelectedIndex - 2;

                // Remove original item
                targetListBox.Items.RemoveAt(targetListBox.SelectedIndex + 2);

                // Enable drawing
                targetListBox.EndUpdate();
            }
        }

        /// <summary>
        /// Moves the selected list box item down.
        /// </summary>
        /// <param name="targetListBox">Target list box.</param>
        private void MoveSelectedListBoxItemDown(ListBox targetListBox)
        {
            // Check there's a selected item AND it isn't the last one
            if (targetListBox.SelectedItem != null && targetListBox.SelectedIndex < targetListBox.Items.Count - 1)
            {
                // Disable drawing
                targetListBox.BeginUpdate();

                // Insert copy below
                targetListBox.Items.Insert(targetListBox.SelectedIndex + 2, targetListBox.Text);

                // Select it
                targetListBox.SelectedIndex = targetListBox.SelectedIndex + 2;

                // Remove original item
                targetListBox.Items.RemoveAt(targetListBox.SelectedIndex - 2);

                // Enable drawing
                targetListBox.EndUpdate();
            }
        }

        /// <summary>
        /// Raises the main form load event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormLoad(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the focused tab page next button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFocusedTabPageNextButtonClick(object sender, EventArgs e)
        {
            // Check there's a further one
            if (this.modulesTabControl.SelectedIndex < this.modulesTabControl.TabCount)
            {
                // Select next tab
                this.modulesTabControl.SelectedIndex = this.modulesTabControl.SelectedIndex + 1;
            }
        }

        /// <summary>
        /// Raises the focused tab page previous button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFocusedTabPagePrevButtonClick(object sender, EventArgs e)
        {
            // Check there's a previous one
            if (this.modulesTabControl.SelectedIndex > 0)
            {
                // Select previous tab
                this.modulesTabControl.SelectedIndex = this.modulesTabControl.SelectedIndex - 1;
            }
        }

        /// <summary>
        /// Raises the modules tab control selected index changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnModulesTabControlSelectedIndexChanged(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the modules tab control draw item event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnModulesTabControlDrawItem(object sender, DrawItemEventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Show about form
        }

        /// <summary>
        /// Raises the loaded up button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoadedUpButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the loaded down button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoadedDownButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the loaded delete button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoadedDeleteButtonClick(object sender, EventArgs e)
        {
            // Set available list box
            ListBox availableListBox = this.GetCurrentAvailableListBox();

            // Set loaded list box
            ListBox loadedListBox = this.GetCurrentLoadedListBox();

            // Iterate selected items
            foreach (string selectedItem in loadedListBox.SelectedItems.OfType<string>().ToList())
            {
                // Deselect from available list
                availableListBox.SetSelected(availableListBox.Items.IndexOf(selectedItem), false);
            }
        }

        /// <summary>
        /// Raises the packs load button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPacksLoadButtonClick(object sender, EventArgs e)
        {
            // Move to last tab (config)
            this.modulesTabControl.SelectedIndex = this.modulesTabControl.TabCount - 1;
        }

        /// <summary>
        /// Raises the config launch button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnConfigLaunchButtonClick(object sender, EventArgs e)
        {
            /* Load modules */

            // Iterate module types
            foreach(string moduleType in this.moduleTypesList)
            {
                // Set list box
                ListBox currentModuleTypeListBox = (ListBox)this.Controls.Find(moduleType + "LoadedListBox", true)[0];

                // Add loaded modules to dictionary in strict order
                for(int i = 0; i < currentModuleTypeListBox.Items.Count; i++)
                {
                    // Set current module
                    IPdBets currentModule = this.rawModulesDictionary[moduleType].Where(m => m.GetType().Namespace == this.sharedCode.DisplayNameToFileName(currentModuleTypeListBox.Items[i].ToString())).ToList()[0];

                    // Add current module
                    this.loadedModulesDictionary[moduleType].Add(currentModule);

                    // Check if it's a GUI module
                    if(currentModule is Form)
                    {
                        // Make it visible
                        ((Form)currentModule).Show();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the config create new pack button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnConfigCreateNewPackButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Gets the current loaded list box.
        /// </summary>
        /// <returns>The current loaded list box.</returns>
        private ListBox GetCurrentLoadedListBox()
        {
            // Return current loaded listbox
            return (ListBox)this.Controls.Find(this.modulesTabControl.SelectedTab.Name.Replace("TabPage", string.Empty) + "LoadedListBox", true)[0];
        }

        /// <summary>
        /// Gets the current available list box.
        /// </summary>
        /// <returns>The current available list box.</returns>
        private ListBox GetCurrentAvailableListBox()
        {
            // Return current available listbox
            return (ListBox)this.Controls.Find(this.modulesTabControl.SelectedTab.Name.Replace("TabPage", string.Empty) + "AvailableListBox", true)[0];
        }

        /// <summary>
        /// Raises the available list box selected index changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAvailableListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            /* Set target list boxes */

            // Available
            ListBox availableListBox = (ListBox)sender;

            // Loaded
            ListBox loadedListBox = this.GetCurrentLoadedListBox();

            /* Process deselected */

            // Check if an item is deselected
            if (this.lastSelectedTabPageItemsDictionary[this.modulesTabControl.SelectedTab.Name].Length >= availableListBox.SelectedItems.Count)
            {
                /* TODO Determine missing item [Perhaps check with LINQ] */

                // Iterate items in last selected dictionary
                foreach (string savedItem in this.lastSelectedTabPageItemsDictionary[this.modulesTabControl.SelectedTab.Name])
                {
                    // Test if current saved item is present in selected items 
                    if (!availableListBox.SelectedItems.Contains(savedItem))
                    {
                        // Present, remove it from loaded list box
                        loadedListBox.Items.Remove(savedItem);

                        // Remove from config modules treeview
                        this.configModulesTreeView.Nodes[this.modulesTabControl.SelectedTab.Name.Replace("TabPage", "Node")].Nodes[savedItem].Remove();

                        // Halt flow
                        break;
                    }
                }
            }
             
            /* Set items collection in dictionary */

            // Redimension array
            this.lastSelectedTabPageItemsDictionary[this.modulesTabControl.SelectedTab.Name] = new string[((ListBox)sender).SelectedItems.Count];

            // Set selected items in proper key
            availableListBox.SelectedItems.CopyTo(this.lastSelectedTabPageItemsDictionary[this.modulesTabControl.SelectedTab.Name], 0);

            /* Add to current loaded list */

            if (((ListBox)sender).SelectedItems.Count > 0)
            {
                // Get selected item text
                string selectedItemText = ((ListBox)sender).SelectedItem.ToString();

                // Check if it's present
                if (!loadedListBox.Items.Contains(selectedItemText))
                {
                    // Set base node name
                    string moduleTypeNodeName = this.modulesTabControl.SelectedTab.Name.Replace("TabPage", "Node");

                    // Add currently-selected item
                    loadedListBox.Items.Add(selectedItemText);

                    // Prepare tree node
                    TreeNode moduleTreeNode = new TreeNode(selectedItemText);

                    // Assign name
                    moduleTreeNode.Name = selectedItemText;

                    // Add to config modules tree view
                    this.configModulesTreeView.Nodes[moduleTypeNodeName].Nodes.Add(moduleTreeNode);

                    // Expand config modules tree view
                    this.configModulesTreeView.ExpandAll();
                }
            }
        }

        /// <summary>
        /// Raises the packs available list box selected index changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPacksAvailableListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the config modules tree view before collapse event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnConfigModulesTreeViewBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            // Prevent collapse
            e.Cancel = true;
        }

        /// <summary>
        /// Raises the packs modules tree view before collapse event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPacksModulesTreeViewBeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            // Prevent collapse
            e.Cancel = true;
        }

        /// <summary>
        /// Raises the always ontop tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAlwaysOntopToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the connector tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnConnectorToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the mechanical tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMechanicalToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the import tool strip menu item drop down item clicked event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnImportToolStripMenuItemDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Raises the new tool strip button click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNewToolStripButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Opens the tool strip button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OpenToolStripButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Saves the tool strip button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void SaveToolStripButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Prints the tool strip button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void PrintToolStripButtonClick(object sender, EventArgs e)
        {
            // Code here
        }

        /// <summary>
        /// Helps the tool strip button click.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void HelpToolStripButtonClick(object sender, EventArgs e)
        {
            // Code here
        }
    }
}