﻿using System;
using System.Collections.Generic;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CompositionSampleGallery
{
    public sealed partial class NavigationFlowSourcePage : Page
    {
        private Compositor _compositor;
        private static int s_persistedItemIndex;

        public NavigationFlowSourcePage()
        {
            InitializeComponent();


            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            var listItems = new List<string>();
            for (int i = 0; i < 300; i++)
            {
                listItems.Add($"Item {i}");
            }
            ItemsGridView.ItemsSource = listItems;

            // Set a fade in animation when this page enters the scene
            var fadeInAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeInAnimation.Target = "Opacity";
            fadeInAnimation.Duration = TimeSpan.FromSeconds(0.3);
            fadeInAnimation.InsertKeyFrame(0, 0);
            fadeInAnimation.InsertKeyFrame(1, 1);

            ElementCompositionPreview.SetImplicitShowAnimation(this, fadeInAnimation);

            // Set a fade out animation when this page exits the scene
            var fadeOutAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeOutAnimation.Target = "Opacity";
            fadeOutAnimation.Duration = TimeSpan.FromSeconds(0.3);
            fadeOutAnimation.InsertKeyFrame(0, 1);
            fadeOutAnimation.InsertKeyFrame(1, 0);

            ElementCompositionPreview.SetImplicitHideAnimation(this, fadeOutAnimation);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ItemsGridView.Loaded += async (o_, e_) =>
                {
                    var connectedAnimation = ConnectedAnimationService
                        .GetForCurrentView()
                        .GetAnimation("BorderDest");
                    if (connectedAnimation != null)
                    {
                        var item = ItemsGridView.Items[s_persistedItemIndex];
                        ItemsGridView.ScrollIntoView(item);
                        await ItemsGridView.TryStartConnectedAnimationAsync(
                            connectedAnimation,
                            item,
                            "BorderSource"
                        );
                    }
                };
            }
        }

        private void ItemsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromSeconds(0.5);

            s_persistedItemIndex = ItemsGridView.Items.IndexOf(e.ClickedItem);
            ItemsGridView.PrepareConnectedAnimation("BorderSource", e.ClickedItem, "BorderSource");

            Frame.Navigate(typeof(NavigationFlowDestinationPage), s_persistedItemIndex);
        }
    }
}
