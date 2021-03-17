using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace WSTowersOffice.App
{
    [Activity(Label = "@string/app_name",Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class SplashActivity : AppCompatActivity
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
                Animate(3000,100);
                Timer tm = new Timer(3100)
                {
                    AutoReset = false
                };
                tm.Elapsed += new ElapsedEventHandler((object sender,ElapsedEventArgs args)=> {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                });
                tm.Start();
            });
            logoImageView.StartAnimation(alphaAnimation);

            // Create your application here
          
        }
        private void Animate(int duration,int delay)
        {
            List<ObjectAnimator> animators = new List<ObjectAnimator>();
            ImageView logoImageView = FindViewById<ImageView>(Resource.Id.logoImg);
            TextView textView = FindViewById<TextView>(Resource.Id.txtWork);
            View buttonRegister = FindViewById(Resource.Id.buttonRegister);
            View buttonLogin= FindViewById(Resource.Id.buttonLogin);
            textView.Alpha = 0;
            buttonLogin.Alpha = 0;
            buttonRegister.Alpha = 0;

            animators.AddRange(GetAnimatorsContent(duration/2, duration / 2, textView));
            animators.AddRange(GetAnimatorsContent(duration, 0, buttonRegister));
            animators.AddRange(GetAnimatorsContent(duration, 0, buttonLogin));
            animators.AddRange(GetAnimatorsLogo(logoImageView, duration, delay));

            foreach (ObjectAnimator animator in animators)
            {
                animator.Start();
            }
        }
        private ObjectAnimator[] GetAnimatorsContent(int duration,int delay, params View[] views) {
            List<ObjectAnimator> animators = new List<ObjectAnimator>();
            foreach (var item in views)
            {
                var alphAnimation = GetAnimatorAlpha(item,duration , null);
                var translateAnimation = GetAnimatorPositionY(item,duration,item.GetY()*0.15f, item.GetY());

                translateAnimation.StartDelay = delay;
                alphAnimation.StartDelay = delay;

                animators.Add(alphAnimation);
                animators.Add(translateAnimation);
                item.SetY(item.GetY() * 0.55f);
            }
            return animators.ToArray();
        } 
        private ObjectAnimator[] GetAnimatorsLogo(ImageView imgView, int duration,int delay) {
            List<ObjectAnimator> logoAnimators = new List<ObjectAnimator>();
            logoAnimators.AddRange(GetAnimatorScale(imgView, duration, new EventHandler((object sender, EventArgs args) => {
                imgView.LayoutParameters.Width = imgView.LayoutParameters.Width / 20 * 13;
                imgView.LayoutParameters.Height = imgView.LayoutParameters.Height / 20 * 13;
            })));
            DisplayMetrics displayMetrics = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            int screenWidth;
            screenWidth = displayMetrics.WidthPixels;
            int[] pos = new int[2];
            imgView.GetLocationOnScreen(pos);
            ObjectAnimator animatorY = GetAnimatorPositionY(imgView, duration, screenWidth*0.85f, imgView.GetY());
            animatorY.StartDelay = delay;
            logoAnimators.Add(animatorY);
            logoAnimators.Add(GetAnimatorAlpha(imgView, duration,null));
            imgView.SetY(screenWidth * 0.85f);

            return logoAnimators.ToArray();
        }
        private ObjectAnimator[] GetAnimatorScale(View @object, int duration, EventHandler? animationEnd)
        {
            ObjectAnimator animatorScaleX = ObjectAnimator.OfFloat(@object, "scaleX", 1f, 0.65f);
            ObjectAnimator animatorScaleY = ObjectAnimator.OfFloat(@object, "scaleY", 1f, 0.65f);
            animatorScaleX.SetDuration(duration);
            animatorScaleY.SetDuration(duration);
            if (animationEnd!=null)
            {
                animatorScaleX.AnimationEnd += animationEnd;
                animatorScaleY.AnimationEnd += animationEnd;
            }
          
            return new ObjectAnimator[] { animatorScaleX, animatorScaleY };
        }
        private ObjectAnimator GetAnimatorAlpha(Java.Lang.Object @object, int duration, EventHandler? animationEnd)
        {
            ObjectAnimator animatorAlpha = ObjectAnimator.OfFloat(@object, "alpha", 0f, 1f);
            animatorAlpha.SetDuration(duration);
            return animatorAlpha;
        }
        private ObjectAnimator GetAnimatorPositionY(View view, int duration,float byY,float toY)
        {
            //ObjectAnimator translateX = ObjectAnimator.OfFloat(view, "translationX", screenHeight / 2f,view.GetX());
            ObjectAnimator translateY = ObjectAnimator.OfFloat(view, "translationY", byY, toY);
            //translateX.SetDuration(duration);
            translateY.SetDuration(duration);
            return  translateY ;
        }
    }
}