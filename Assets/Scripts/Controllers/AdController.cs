using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Controllers
{
    public class AdController : MonoBehaviour
    {

        private InterstitialAd interstitialAd;
        private RewardedInterstitialAd rewardedInterstitialAd;

        private string rewardedInterstitialAdId = "ca-app-pub-5136043887247668/6779231075";
        private string interstitialAdId = "ca-app-pub-5136043887247668/8092312740";

        public static AdController instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        void Start()
        {
            MobileAds.Initialize(InitializationStatus => { });
            this.RequestInterstitial();
            this.RequestRewarded();

            this.interstitialAd.OnAdClosed += InterstitialAd_OnAdClosed;
            this.interstitialAd.OnAdFailedToLoad += InterstitialAd_OnAdFailedToLoad;
        }

        private void InterstitialAd_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            RequestInterstitial();
        }

        private void InterstitialAd_OnAdClosed(object sender, EventArgs e)
        {
            RequestInterstitial();
        }

        private void RequestRewarded()
        {
            RewardedInterstitialAd.LoadAd(rewardedInterstitialAdId, CreateAdRequest(), adLoadCallback);
        }

        private void adLoadCallback(RewardedInterstitialAd ad, AdFailedToLoadEventArgs error)
        {
            if (error == null)
            {
                rewardedInterstitialAd = ad;
            }
        }
        public void ShowRewardedInterstitialAd()
        {
            if (rewardedInterstitialAd != null)
            {
                rewardedInterstitialAd.Show(userEarnedRewardCallback);
            }
        }
        private void userEarnedRewardCallback(Reward reward)
        {
            Console.WriteLine(reward.Amount);
        }
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }
        private void RequestInterstitial()
        {
            if (this.interstitialAd != null)
                this.interstitialAd.Destroy();

            this.interstitialAd = new InterstitialAd(interstitialAdId);
            this.interstitialAd.LoadAd(CreateAdRequest());
        }

        public void ShowInterstitial()
        {
            if (this.interstitialAd.IsLoaded())
            {
                this.interstitialAd.Show();
            }
            else
            {
                Debug.Log("interstitial not loaded");
            }
        }
    }
}
