﻿using ImageSorter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ImageSorter
{
    public partial class Form1 : Form
    {
        String listbox1Entry;
        bool movetest = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                String dir = folderBrowserDialog1.SelectedPath;
                String[] extensionList = { "*.jpg", "*.png", "*.bmp" };
                listBox1.Items.Clear();
                foreach (String fileExtension in extensionList)
                {
                    foreach (String file in Directory.GetFiles(dir, fileExtension, SearchOption.TopDirectoryOnly))
                    {
                        listBox1.Items.Add(file);
                    }
                }
            }

        }

        private void RenameFiles()
        {
            // Get the current directory
            string directory = Directory.GetCurrentDirectory();

            // Get the file extensions to filter
            string[] extensions = { ".jpg", ".png", ".bmp" };

            // Get the files in the directory with the selected extensions
            var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
                .Where(file => extensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));

            // Rename the files in sequential order
            int count = 1;
            foreach (var item in listBox1.Items)
            {
                // Get the old and new file names
                string oldFileName = Path.GetFileName(item.ToString());
                string newFileName = $"{count}{Path.GetExtension(oldFileName)}";

                // Rename the file if it exists in the directory
                if (files.Contains(Path.Combine(directory, oldFileName)))
                {
                    File.Move(Path.Combine(directory, oldFileName), Path.Combine(directory, newFileName));
                }

                // Increment the count
                count++;
            }
        }

        private Point _lastMouseDownLocation;

        private void ListBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _lastMouseDownLocation = e.Location;
        }

        private void ListBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int distance = Math.Abs(e.Location.X - _lastMouseDownLocation.X) + Math.Abs(e.Location.Y - _lastMouseDownLocation.Y);
                if (distance > 5)
                {
                    DoDragDrop(listBox1.SelectedItem, DragDropEffects.Move);
                }
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            movetest = true;
            listBox1.MoveSelectedItemDown();
            pictureBox1.Load(listbox1Entry);
            movetest = false;
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            movetest = true;
            listBox1.MoveSelectedItemUp();
            pictureBox1.Load(listbox1Entry);
            movetest = false;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listbox1Entry = listBox1.GetItemText(listBox1.SelectedItem);
            this.Name = listbox1Entry;
            this.Text = listbox1Entry;
            pictureBox1.ImageLocation = listbox1Entry;
            if (movetest == false)
            {
                pictureBox1.Load(listbox1Entry);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            // Create the destination folder
            string destFolder = Path.Combine(Directory.GetCurrentDirectory(), "SortedImages");
            Directory.CreateDirectory(destFolder);

            // Loop through the files in the list box and copy them to the destination folder
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                string sourceFile = listBox1.Items[i].ToString();
                string destFile = Path.Combine(destFolder, $"image{i + 1}.png");

                if (File.Exists(sourceFile))
                {
                    try
                    {
                        File.Copy(sourceFile, destFile, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying file {sourceFile}: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show($"File {sourceFile} does not exist");
                }
            }

            MessageBox.Show("Files copied successfully");
        }
    }

    public static class ListBoxExtension
    {
        public static void MoveSelectedItemUp(this ListBox listBox)
        {
            MoveSelectedItem(listBox, -1);
        }

        public static void MoveSelectedItemDown(this ListBox listBox)
        {
            MoveSelectedItem(listBox, 1);
        }

        static void MoveSelectedItem(ListBox listBox, int direction)
        {
            // Checking selected item
            if (listBox.SelectedItem == null || listBox.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = listBox.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= listBox.Items.Count)
                return; // Index out of range - nothing to do

            object selected = listBox.SelectedItem;

            // Save checked state if it is applicable
            var checkedListBox = listBox as CheckedListBox;
            var checkState = CheckState.Unchecked;
            if (checkedListBox != null)
                checkState = checkedListBox.GetItemCheckState(checkedListBox.SelectedIndex);

            // Removing removable element
            listBox.Items.Remove(selected);
            // Insert it in new position
            listBox.Items.Insert(newIndex, selected);
            // Restore selection
            listBox.SetSelected(newIndex, true);

            // Restore checked state if it is applicable
            if (checkedListBox == null)
                return;
            checkedListBox.SetItemCheckState(newIndex, checkState);
        }
    }
}
