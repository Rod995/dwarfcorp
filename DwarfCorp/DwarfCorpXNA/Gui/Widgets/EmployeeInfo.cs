#define ENABLE_CHAT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DwarfCorp.Gui;
using DwarfCorp.Gui.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DwarfCorp.Gui.Widgets
{

    public class EmployeeInfo : Widget
    {
        public bool EnablePosession = false;
        private CreatureAI _employee;
        public CreatureAI Employee
        {
            get { return _employee; }
            set { _employee = value;  Invalidate(); }
        }

        private Widget InteriorPanel;

        private DwarfCorp.Gui.Widgets.EmployeePortrait Icon;
        private Widget NameLabel;

        private Widget StatDexterity;
        private Widget StatStrength;
        private Widget StatWisdom;
        private Widget StatCharisma;
        private Widget StatConstitution;
        private Widget StatIntelligence;
        private Widget StatSize;

        private Gui.Widgets.TextProgressBar Hunger;
        private Gui.Widgets.TextProgressBar Energy;
        private Gui.Widgets.TextProgressBar Happiness;
        private Gui.Widgets.TextProgressBar Health;
        private Gui.Widgets.TextProgressBar Boredom;

        private Widget TitleEditor;
        private Widget LevelLabel;
        private Widget PayLabel;
        private Widget LevelButton;

        private Widget Thoughts;
        private Widget Bio;
        private Widget TaskLabel;
        private Widget CancelTask;
        private Widget AgeLabel;

        public Action<Widget> OnFireClicked;

        public override void Construct()
        {
            Text = "You have no employees.";
            Font = "font16";

            InteriorPanel = AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockFill,
                Hidden = true,
                Background = new TileReference("basic", 0),
                Font = "font8",
            });

            var top = InteriorPanel.AddChild(new Gui.Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 96)
            });

            Icon = top.AddChild(new DwarfCorp.Gui.Widgets.EmployeePortrait
            {
                AutoLayout = AutoLayout.DockLeft,
                MinimumSize = new Point(48, 40),
            }) as EmployeePortrait;        

            NameLabel = top.AddChild(new Gui.Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 48),
                Font = "font16"
            });

            var levelHolder = top.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(256, 24)
            });
            TitleEditor = levelHolder.AddChild(new Gui.Widgets.EditableTextField()
            {
                AutoLayout = AutoLayout.DockLeft,
                MinimumSize = new Point(128, 24),
                OnTextChange = (sender) =>
                {
                    Employee.Stats.Title = sender.Text;
                },
                Tooltip = "Employee title. You can customize this."
            });

            LevelLabel = levelHolder.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockLeft,
                MinimumSize = new Point(128, 24)
            });

            var columns = InteriorPanel.AddChild(new Gui.Widgets.Columns
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 100),
                ColumnCount = 3
            });
            
            var left = columns.AddChild(new Gui.Widget());
            var right = columns.AddChild(new Gui.Widget());
            var evenMoreRight = columns.AddChild(new Gui.Widget());
            #region Stats
            var statParent = left.AddChild(new Gui.Widgets.Columns
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 60)
            });

            var statsLeft = statParent.AddChild(new Widget());
            var statsRight = statParent.AddChild(new Widget());

            StatDexterity = statsLeft.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Dexterity (affects dwarf speed)"
            });

            StatStrength = statsLeft.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Strength (affects dwarf attack power)"
            });

            StatWisdom = statsLeft.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Wisdom (affects temprement and spell resistance)"
            });

            StatCharisma = statsLeft.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Charisma (affects ability to make friends)"
            });

            StatConstitution = statsRight.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Constitution (affects dwarf health and damage resistance)"
            });

            StatIntelligence = statsRight.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Intelligence (affects crafting/farming)"
            });

            StatSize = statsRight.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 16),
                Tooltip = "Size"
            });
            #endregion

            #region status bars
            Hunger = CreateStatusBar(right, "Hunger", "Starving", "Hungry", "Peckish", "Okay");
            Energy = CreateStatusBar(right, "Energy", "Exhausted", "Tired", "Okay", "Energetic");
            Happiness = CreateStatusBar(right, "Happiness", "Miserable", "Unhappy", "So So", "Happy", "Euphoric");
            Health = CreateStatusBar(evenMoreRight, "Health", "Near Death", "Critical", "Hurt", "Uncomfortable", "Fine", "Perfect");
            Boredom = CreateStatusBar(evenMoreRight, "Boredom", "Desperate", "Overworked", "Bored", "Meh", "Fine", "Excited");
            #endregion           

            PayLabel = InteriorPanel.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 24)
            });
            
            AgeLabel = InteriorPanel.AddChild(new Widget() {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 24)
            });

            Bio = InteriorPanel.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 24)
            });

            var task = InteriorPanel.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(1, 24)
            });

            var inventory = task.AddChild(new Button
            {
                AutoLayout = AutoLayout.DockRight,
                Text = "Backpack...",
                OnClick = (sender, args) =>
                {
                    var employeeInfo = sender.Parent.Parent.Parent as EmployeeInfo;
                    if (employeeInfo != null && employeeInfo.Employee != null)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Backpack contains:\n");
                        Dictionary<string, ResourceAmount> aggregateResources = employeeInfo.Employee.Creature.Inventory.Aggregate();
                        foreach (var resource in aggregateResources)
                        {
                            stringBuilder.Append(String.Format("{0}x {1}\n", resource.Value.NumResources, resource.Key));
                        }
                        if (aggregateResources.Count == 0)
                        {
                            stringBuilder.Append("Nothing.");
                        }

                        Confirm popup = new Confirm()
                        {
                            CancelText = "",
                            Text = stringBuilder.ToString()
                        };


                        sender.Root.ShowMinorPopup(popup);

                        if (aggregateResources.Count > 0)
                        {
                            popup.AddChild(new Button()
                            {
                                Text = "Empty",
                                Tooltip = "Click to order this dwarf to empty their backpack.",
                                AutoLayout = AutoLayout.FloatBottomLeft,
                                OnClick = (currSender, currArgs) =>
                                {
                                    if (employeeInfo != null && employeeInfo.Employee != null
                                         && employeeInfo.Employee.Creature != null)
                                        employeeInfo.Employee.Creature.RestockAllImmediately(true);
                                }
                            });
                            popup.Layout();

                        }
                       
                    }
                }
            });

            CancelTask = task.AddChild(new Button
            {
                AutoLayout = AutoLayout.DockRight,
                Text = "Cancel Task",
                ChangeColorOnHover = true
            });

            TaskLabel = task.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockRight,
                MinimumSize = new Point(256, 24),
                TextVerticalAlign = VerticalAlign.Center,
                TextHorizontalAlign = HorizontalAlign.Right
            });

            Thoughts = InteriorPanel.AddChild(new Widget
            {
                AutoLayout = AutoLayout.DockTop,
                MinimumSize = new Point(0, 24)
            });

            var bottomBar = InteriorPanel.AddChild(new Widget
            {
                Transparent = true,
                AutoLayout = AutoLayout.DockBottom,
                MinimumSize = new Point(0, 32)
            });

            bottomBar.AddChild(new Button()
            {
                Text = "Fire",
                Border = "border-button",
                AutoLayout = AutoLayout.DockRight,
                OnClick = (sender, args) =>
                {
                    Root.SafeCall(OnFireClicked, this);
                }
            });

            /*
            if (EnablePosession)
            {
                bottomBar.AddChild(new Button()
                {
                    Text = "Follow",
                    Tooltip = "Click to directly control this dwarf and have the camera follow.",
                    AutoLayout = AutoLayout.DockRight,
                    OnClick = (sender, args) =>
                    {
                        (sender.Parent.Parent.Parent as EmployeeInfo).Employee.World.Tutorial("dwarf follow");
                        (sender.Parent.Parent.Parent as EmployeeInfo).Employee.IsPosessed = true;
                    }
                });
            }
            */

            bottomBar.AddChild(new Button()
            {
                Text = "Tasks...",
                Tooltip = "Open allowed tasks filter.",
                AutoLayout = AutoLayout.DockRight,
                OnClick = (sender, args) =>
                {
                    var screen = sender.Root.RenderData.VirtualScreen;
                    sender.Root.ShowModalPopup(new AllowedTaskFilter
                    {
                        Employee = Employee,
                        Tag = "selected-employee-allowable-tasks",
                        AutoLayout = AutoLayout.DockFill,
                        MinimumSize = new Point(256, 256),
                        Border = "border-fancy",
                        Rect = new Rectangle(screen.Center.X - 128, screen.Center.Y - 128, 256, 256)
                    });
                }
            });

#if ENABLE_CHAT
            if (Employee != null && Employee.GetRoot().GetComponent<DwarfThoughts>() != null)
            {
                bottomBar.AddChild(new Button()
                {
                    Text = "Chat...",
                    Tooltip = "Have a talk with your employee.",
                    AutoLayout = AutoLayout.DockRight,
                    OnClick = (sender, args) =>
                    {
                        Employee.Chat();
                    }
                });
            }
#endif


            LevelButton = bottomBar.AddChild(new Button()
            {
                Text = "Promote!",
                Border = "border-button",
                AutoLayout = AutoLayout.DockRight,
                Tooltip = "Click to promote this dwarf.\nPromoting Dwarves raises their pay and makes them\nmore effective workers.",
                OnClick = (sender, args) =>
                {
                    var prevLevel = Employee.Stats.CurrentLevel;
                    Employee.Stats.LevelUp();
                    if (Employee.Stats.CurrentLevel.HealingPower > prevLevel.HealingPower)
                    {
                        Employee.World.MakeAnnouncement(String.Format("{0}'s healing power increased to {1}!", Employee.Stats.FullName, Employee.Stats.CurrentLevel.HealingPower));
                    }

                    if (Employee.Stats.CurrentLevel.ExtraAttacks.Count > prevLevel.ExtraAttacks.Count)
                    {
                        Employee.World.MakeAnnouncement(String.Format("{0} learned to cast {1}!", Employee.Stats.FullName, Employee.Stats.CurrentLevel.ExtraAttacks.Last().Name));
                    }
                    SoundManager.PlaySound(ContentPaths.Audio.change, 0.5f);
                    Invalidate();
                    Employee.Creature.AddThought(Thought.ThoughtType.GotPromoted);
                }
            });


            var topbuttons = top.AddChild(new Widget()
            {
                AutoLayout = AutoLayout.FloatTopRight,
                MinimumSize = new Point(32, 24)
            });
            topbuttons.AddChild(new Widget()
            {
                Text = "<",
                Font = "font10",
                Tooltip = "Previous employee.",
                AutoLayout = AutoLayout.DockLeft,
                ChangeColorOnHover = true,
                MinimumSize = new Point(16, 24),
                OnClick = (sender, args) =>
                {
                    if (Employee == null)
                        return;
                    int idx = Employee.Faction.Minions.IndexOf(Employee);
                    if (idx < 0)
                        return;
                    idx--;
                    Employee = Employee.Faction.Minions[Math.Abs(idx) % Employee.Faction.Minions.Count];
                    Employee.World.Master.SelectedMinions = new List<CreatureAI>() { Employee };
                }
            });
            topbuttons.AddChild(new Widget()
            {
                Text = ">",
                Font = "font10",
                Tooltip = "Next employee.",
                AutoLayout = AutoLayout.DockRight,
                ChangeColorOnHover = true,
                MinimumSize = new Point(16, 24),
                OnClick = (sender, args) =>
                {
                    if (Employee == null)
                        return;
                    int idx = Employee.Faction.Minions.IndexOf(Employee);
                    if (idx < 0)
                        return;
                    idx++;
                    Employee = Employee.Faction.Minions[idx % Employee.Faction.Minions.Count];
                    Employee.World.Master.SelectedMinions = new List<CreatureAI>() { Employee };
                }
            });

            base.Construct();
        }

        private Gui.Widgets.TextProgressBar CreateStatusBar(Widget AddTo, String Label, params String[] PercentageLabels)
        {
            return AddTo.AddChild(new Gui.Widgets.TextProgressBar
            {
                AutoLayout = AutoLayout.DockTop,
                Label = Label,
                SegmentCount = 10,
                PercentageLabels = PercentageLabels,
                Font = "font8"
            }) as Gui.Widgets.TextProgressBar;
        }

        protected override Gui.Mesh Redraw()
        {
            // Set values from CreatureAI
            if (Employee != null && !Employee.IsDead)
            {
                InteriorPanel.Hidden = false;

                //var idx = EmployeePanel.GetIconIndex(Employee.Stats.CurrentClass.Name);
                //Icon.Background = idx >= 0 ? new TileReference("dwarves", idx) : null;
                //Icon.Invalidate();
                //Icon.Sprite = Employee.Creature.Sprite.Animations[0];
                var sprite = Employee.GetRoot().GetComponent<LayeredSprites.LayeredCharacterSprite>();
                if (sprite != null)
                {
                    Icon.Sprite = sprite.GetLayers();
                    Icon.AnimationPlayer = sprite.AnimPlayer;
                }
                else
                {
                    Icon.Sprite = null;
                    Icon.AnimationPlayer = null;
                }

                NameLabel.Text = "\n" + Employee.Stats.FullName;
                StatDexterity.Text = String.Format("Dex: {0}", Employee.Stats.BuffedDex);
                StatStrength.Text = String.Format("Str: {0}", Employee.Stats.BuffedStr);
                StatWisdom.Text = String.Format("Wis: {0}", Employee.Stats.BuffedWis);
                StatConstitution.Text = String.Format("Con: {0}", Employee.Stats.BuffedCon);
                StatIntelligence.Text = String.Format("Int: {0}", Employee.Stats.BuffedInt);
                StatSize.Text = String.Format("Size: {0}", Employee.Stats.BuffedSiz);
                StatCharisma.Text = String.Format("Cha: {0}", Employee.Stats.BuffedChar);
                SetStatusBar(Hunger, Employee.Status.Hunger);
                SetStatusBar(Energy, Employee.Status.Energy);
                SetStatusBar(Happiness, Employee.Status.Happiness);
                SetStatusBar(Health, Employee.Status.Health);
                SetStatusBar(Boredom, Employee.Status.Boredom);
                TitleEditor.Text = Employee.Stats.Title ?? Employee.Stats.CurrentClass.Name;
                LevelLabel.Text = String.Format("Level {0} {1}\n({2} xp). {3}",
                    Employee.Stats.LevelIndex,
                    Employee.Stats.CurrentClass.Name,
                    Employee.Stats.XP,
                    Employee.Creature.Stats.Gender);

                Bio.Text = Employee.Biography;

                StringBuilder thoughtsBuilder = new StringBuilder();
                thoughtsBuilder.Append("Thoughts:\n");
                var thoughts = Employee.Physics.GetComponent<DwarfThoughts>();
                if (thoughts != null)
                    foreach (var thought in thoughts.Thoughts)
                        thoughtsBuilder.Append(String.Format("{0} ({1})\n", thought.Description, thought.HappinessModifier));
   
                var diseases = Employee.Creature.Buffs.OfType<Disease>();
                if (diseases.Any())
                {
                    thoughtsBuilder.Append("Conditions: ");
                }

                if (Employee.Status.IsAsleep)
                {
                    thoughtsBuilder.AppendLine("Unconscious");
                }

                foreach (var disease in diseases)
                {
                    thoughtsBuilder.AppendLine(disease.Name);
                }
                Thoughts.Text = thoughtsBuilder.ToString();

                if (Employee.Stats.CurrentClass.Levels.Count > Employee.Stats.LevelIndex + 1)
                {
                    var nextLevel = Employee.Stats.CurrentClass.Levels[Employee.Stats.LevelIndex + 1];
                    var diff = nextLevel.XP - Employee.Stats.XP;

                    if (diff > 0)
                    {
                        //ExperienceLabel.Text = String.Format("XP: {0}\n({1} to next level)",
                        //    Employee.Stats.XP, diff);
                        LevelButton.Hidden = true;
                        LevelButton.Invalidate();
                    }
                    else
                    {
                        //ExperienceLabel.Text = String.Format("XP: {0}\n({1} to next level)",
                        //    Employee.Stats.XP, "(Overqualified)");
                        LevelButton.Hidden = false;
                        LevelButton.Tooltip = "Promote to " + nextLevel.Name;
                        LevelButton.Invalidate();
                    }
                }
                else
                {
                    //ExperienceLabel.Text = String.Format("XP: {0}", Employee.Stats.XP);
                }

                PayLabel.Text = String.Format("Pay: {0}/day\nWealth: {1}", Employee.Stats.CurrentLevel.Pay,
                    Employee.Status.Money);

                if (Employee.CurrentTask != null)
                {
                    TaskLabel.Text = "Current Task: " + Employee.CurrentTask.Name;
                    CancelTask.TextColor = new Vector4(0, 0, 0, 1);
                    CancelTask.Invalidate();
                    CancelTask.OnClick = (sender, args) =>
                    {
                        if (Employee.CurrentTask != null)
                        {
                            Employee.CancelCurrentTask();
                            TaskLabel.Text = "No tasks";
                            TaskLabel.Invalidate();
                            CancelTask.OnClick = null;
                            CancelTask.TextColor = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                            CancelTask.Invalidate();
                        }
                    };
                }
                else
                {
                    TaskLabel.Text = "No tasks";
                    CancelTask.OnClick = null;
                    CancelTask.TextColor = new Vector4(0.5f, 0.5f, 0.5f, 0.5f);
                    CancelTask.Invalidate();
                }

                AgeLabel.Text = String.Format("Age: {0}", Employee.Stats.Age);
            }
            else
                InteriorPanel.Hidden = true;

            foreach (var child in Children)
                child.Invalidate();

            return base.Redraw();
        }

        private void SetStatusBar(Gui.Widgets.TextProgressBar Bar, Status Status)
        {
            Bar.Percentage = (float)Status.Percentage / 100.0f;
        }
    }
}
