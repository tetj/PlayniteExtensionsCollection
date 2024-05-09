﻿using Newtonsoft.Json;
using FlowHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YouTubeCommon.Models;

namespace YouTubeCommon
{
    public class YouTube
    {
        private const string youtubeResponseRegexStr = @"var ytInitialData = ((.*?(?=(;<\/script>))))";
        private const string youtubeSearchTemplateShort = @"https://www.youtube.com/results?search_query={0}&sp=EgQQARgB";
        private const string youtubeSearchTemplate = @"https://www.youtube.com/results?search_query={0}";

        public static List<YoutubeSearchItem> GetYoutubeSearchResults(string searchTerm, bool searchShortVideos)
        {
            var uri = string.Empty;
            if (searchShortVideos)
            {
                uri = string.Format(youtubeSearchTemplateShort, Uri.EscapeDataString(searchTerm));
            }
            else
            {
                uri = string.Format(youtubeSearchTemplate, Uri.EscapeDataString(searchTerm));
            }

            var downloadedString = HttpRequestFactory.GetFlowHttpRequest().WithUrl(uri).DownloadString();
            if (downloadedString.IsSuccess)
            {
                var embeddedJsonMatch = Regex.Match(downloadedString.Content, youtubeResponseRegexStr);
                if (embeddedJsonMatch.Success)
                {
                    var response = JsonConvert.DeserializeObject<YoutubeEmbeddedResponse>(embeddedJsonMatch.Groups[1].Value);
                    var itemSection = response.Contents.TwoColumnSearchResultsRenderer.PrimaryContents.SectionListRenderer.Contents[0].ItemSectionRenderer.Contents;
                    if (itemSection.Count < 13)
                    {
                        return itemSection.Where(x => x.VideoRenderer != null).Select(x => ItemToYoutubeSearchObj(x)).ToList();
                    }
                    else
                    {
                        return itemSection.Where(x => x.VideoRenderer != null).Take(14).Select(x => ItemToYoutubeSearchObj(x)).ToList();
                    }

                }
            }

            return new List<YoutubeSearchItem>();
        }

        public static YoutubeSearchItem ItemToYoutubeSearchObj(ItemSectionRendererContent item)
        {
            return new YoutubeSearchItem
            {
                ThumbnailUrl = item.VideoRenderer.Thumbnail.Thumbnails.OrderBy(i => i.Width).FirstOrDefault()?.Url,
                VideoTitle = item.VideoRenderer.Title.Runs[0].Text,
                VideoId = item.VideoRenderer.VideoId,
                // For some reason some videos don't report the lenght
                VideoLenght = item.VideoRenderer.LengthText?.SimpleText ?? "-",
                ChannelName = item.VideoRenderer.OwnerText.Runs[0].Text
            };
        }

    }
}