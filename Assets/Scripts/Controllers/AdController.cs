using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdController : MonoBehaviour
{

    private InterstitialAd interstitialAd;
    private RewardedInterstitialAd rewardedInterstitialAd;

    private string rewardedAdId = "ca-app-pub-3940256099942544/5354046379";
    private string interstitialAdId = "ca-app-pub-3940256099942544/10331737121";

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
        RewardedInterstitialAd.LoadAd(rewardedAdId, CreateAdRequest(), adLoadCallback);
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
