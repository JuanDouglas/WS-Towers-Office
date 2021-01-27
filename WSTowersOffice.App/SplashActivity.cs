using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WSTowersOffice.App
{
    [Activity(Label = "SplashActivity",Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_splash);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            ImageView logoImageView = FindViewById<ImageView>(Resource.Id.logoImg);
            AlphaAnimation alphaAnimation = new AlphaAnimation(0f, 1f)
            {
                Duration = 3000
            };
            alphaAnimation.AnimationEnd += new EventHandler<Animation.AnimationEndEventArgs>((object sender, Animation.AnimationEndEventArgs args) => {
                SetContentView(Resource.Layout.activity_start);
                ImageView logoImageView = FindViewById<ImageView>(Resource.Id.logoImg);
                AnimateLogo(logoImageView, 3000);
                //Intent intent = new Intent(this, typeof(MainActivity));
                //StartActivity(intent);
                //Finish();
            });
            logoImageView.StartAnimation(alphaAnimation);
            // Create your application here
        }
        private void AnimateLogo(ImageView imgview, int duration)
        {
            List<ObjectAnimator> logoAnimators = new List<ObjectAnimator>();
            logoAnimators.AddRange(GetAnimatorScale(imgview, duration, new EventHandler((object sender, EventArgs args) => {
                imgview.LayoutParameters.Width = imgview.LayoutParameters.Width / 20 * 13;
                imgview.LayoutParameters.Height = imgview.LayoutParameters.Height / 20 * 13;
            })));
            logoAnimators.AddRange(GetAnimatorPosition(imgview, duration));
            logoAnimators.Add(GetAnimatorAlpha(imgview, duration));
            foreach (var item in logoAnimators)
            {
                item.Start();
            }
        }
        private ObjectAnimator[] GetAnimatorScale(View @object, int duration, EventHandler animationEnd)
        {
            ObjectAnimator animatorScaleX = ObjectAnimator.OfFloat(@object, "scaleX", 1f, 0.65f);
            ObjectAnimator animatorScaleY = ObjectAnimator.OfFloat(@object, "scaleY", 1f, 0.65f);
            animatorScaleX.SetDuration(duration);
            animatorScaleY.SetDuration(duration);
            animatorScaleX.AnimationEnd += animationEnd;
            animatorScaleY.AnimationEnd += animationEnd;
            return new ObjectAnimator[] { animatorScaleX, animatorScaleY };
        }
        private ObjectAnimator GetAnimatorAlpha(Java.Lang.Object @object, int duration)
        {
            ObjectAnimator animatorAlpha = ObjectAnimator.OfFloat(@object, "alpha", 0f, 1f);
            animatorAlpha.SetDuration(duration);
            return animatorAlpha;
        }
        private ObjectAnimator[] GetAnimatorPosition(View view, int duration)
        {
            DisplayMetrics displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int screenWidth;
            screenWidth = displayMetrics.WidthPixels;
            int[] pos = new int[2];
            view.GetLocationOnScreen(pos);
            //ObjectAnimator translateX = ObjectAnimator.OfFloat(view, "translationX", screenHeight / 2f,view.GetX());
            ObjectAnimator translateY = ObjectAnimator.OfFloat(view, "translationY", screenWidth / 2f, view.GetY());
            //translateX.SetDuration(duration);
            translateY.SetDuration(duration);
            return new ObjectAnimator[] { translateY };
        }
    }
}