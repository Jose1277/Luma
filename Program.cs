using Luma.Entities;
using Luma.Entities.Services;
using System;

namespace Luma
{
    class Program
    { 
        private static Dictionary<string, ProbeSyncData> probesSyncData = new Dictionary<string, ProbeSyncData>();
        public static async Task Main(string[] args)
        {
            const string username = "jose1277";
            const string email = "josee.oliveira022@gmail.com";

            ApiService apiService = new ApiService();
            var startApiResponse = await apiService.StartApi(username, email);

            if(startApiResponse.Code == "Success")
            {
                await Console.Out.WriteLineAsync($"Token de acesso: {startApiResponse.AccessToken}");
                try
                {
                    var probesResponse = await apiService.ListProbes();
                    if (probesResponse.Code == "Success")
                    {
                        foreach (var probe in probesResponse.Probes)
                        {
                            const long _fiveMilliseconds = 50000;
                            long timeOffset = 0;
                            long newtimeOffset = 0;
                            long roundTripTime = 0;
                            await Console.Out.WriteLineAsync($"\nId: {probe.Id}\nNome: {probe.Name}\nEncoding: {probe.Encoding}\n");
                            do
                            {
                                long t0 = DateTimeOffset.UtcNow.Ticks + timeOffset;
                                var probeSyncResponse = await apiService.SyncProbe(probe.Id);
                                long t3 = DateTimeOffset.UtcNow.Ticks + timeOffset;
                                if (probeSyncResponse.Code == "Success")
                                {
                                    long t1 = Entities.Calculos.DecodeTimestamp(probeSyncResponse.t1, probe.Encoding);
                                    long t2 = Entities.Calculos.DecodeTimestamp(probeSyncResponse.t2, probe.Encoding);

                                    DateTimeOffset t0Date = new DateTimeOffset(new DateTime(t0, DateTimeKind.Utc));
                                    DateTimeOffset t1Date = new DateTimeOffset(new DateTime(t1, DateTimeKind.Utc));
                                    DateTimeOffset t2Date = new DateTimeOffset(new DateTime(t2, DateTimeKind.Utc));
                                    DateTimeOffset t3Date = new DateTimeOffset(new DateTime(t3, DateTimeKind.Utc));

                                    await Console.Out.WriteLineAsync($"t0: {t0Date}\nt1: {t1Date}\nt2: {t2Date}\nt3: {t3Date}");

                                    newtimeOffset = (t1 - t0 + (t2 - t3))/2;
                                    roundTripTime = t3 - t0 - (t2 - t1);
                                    timeOffset += newtimeOffset;

                                    await Console.Out.WriteLineAsync($"Offset: {newtimeOffset}\nRoundTripTime: {roundTripTime}\nTotalOffset: {timeOffset}\n");  
                                }
                                else
                                {
                                    await Console.Out.WriteLineAsync($"Erro: {probeSyncResponse.Message}");
                                    break;
                                }
                            }
                            while (Math.Abs(newtimeOffset) > _fiveMilliseconds);
                            await Console.Out.WriteLineAsync($"Probe {probe.Name}");
                            ProbeSyncData data = new ProbeSyncData
                            {
                                ProbeName = probe.Name,
                                TimeOffset = timeOffset,
                                LastTimeOffset = newtimeOffset,
                                RoundTripTime = roundTripTime,
                                Encoding = probe.Encoding
                            };

                            probesSyncData[probe.Name] = data;
                        }

                        try
                        {
                            var jobResponse = await apiService.takeJob();
                            await Console.Out.WriteLineAsync("Taking job");
                            while (jobResponse.Code != null)
                            { 
                                var job = jobResponse.Job;
                                var probeSyncData = probesSyncData[job.ProbeName];
                                long ProbeNow = DateTimeOffset.UtcNow.Ticks + probeSyncData.TimeOffset;
                                string probeNowEncoded = Calculos.EncodeTimestamp(ProbeNow, probeSyncData.Encoding);
                                await Console.Out.WriteLineAsync($"Checking {job.ProbeName}");
                                var checkJobResponse = await apiService.checkJob(job.Id, probeNowEncoded, probeSyncData.RoundTripTime);
                                if (checkJobResponse.Code == "Done")
                                {
                                    await Console.Out.WriteLineAsync($"Job {job.Id} finalizado com sucesso.");
                                    break;
                                }
                                else if (checkJobResponse.Code != "Success")
                                {
                                    await Console.Out.WriteLineAsync($"Erro: {checkJobResponse.Message}");
                                    if (checkJobResponse.Code == "Error")
                                    {
                                        continue;
                                    }
                                    else if (checkJobResponse.Code == "Fail")
                                    {
                                        return;
                                    }
                                }
                                jobResponse = await apiService.takeJob();
                            }
                        }
                        catch (Exception ex)
                        {
                            await Console.Out.WriteLineAsync($"Erro: {ex.Message}");    
                        }
                    }
                    else
                    {
                        await Console.Out.WriteLineAsync($"Erro: {probesResponse.Message}");
                    }
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Erro: {ex.Message}");
                }
            }
            else
            {
                await Console.Out.WriteLineAsync($"Erro: {startApiResponse.Message}");
            }
        }
    }
}