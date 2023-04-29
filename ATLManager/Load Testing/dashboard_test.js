import { sleep, group } from 'k6'
import http from 'k6/http'

export const options = {
  ext: {
    loadimpact: {
      distribution: { 'amazon:us:ashburn': { loadZone: 'amazon:us:ashburn', percent: 100 } },
      apm: [],
    },
  },
  thresholds: {},
  scenarios: {
    Scenario_1: {
      executor: 'ramping-vus',
      gracefulStop: '30s',
      stages: [
        { target: 5, duration: '1m' },
        { target: 20, duration: '1m' },
        { target: 100, duration: '3m30s' },
        { target: 20, duration: '1m' },
        { target: 0, duration: '1m' },
      ],
      gracefulRampDown: '30s',
      exec: 'scenario_1',
    },
  },
}

export function scenario_1() {
  let response

  group('page_1 - http://localhost:56970/', function () {
    response = http.get('http://localhost:56970/', {
      headers: {
        'upgrade-insecure-requests': '1',
        'sec-ch-ua': '"Not?A_Brand";v="99", "Opera GX";v="97", "Chromium";v="111"',
        'sec-ch-ua-mobile': '?0',
        'sec-ch-ua-platform': '"Windows"',
      },
    })
    response = http.get(
      'http://localhost:59253/093faedd5d8f4691aa1d873070b081ab/browserLinkSignalR/negotiate?requestUrl=http%3A%2F%2Flocalhost%3A56970%2F&browserName=&userAgent=Mozilla%2F5.0+(Windows+NT+10.0%3B+Win64%3B+x64)+AppleWebKit%2F537.36+(KHTML%2C+like+Gecko)+Chrome%2F111.0.0.0+Safari%2F537.36+OPR%2F97.0.0.0&browserIdKey=window.browserLink.initializationData.browserId&browserId=4d69-d3b9&clientProtocol=1.3&_=1682624609542',
      {
        headers: {
          accept: 'text/plain, */*; q=0.01',
          'content-type': 'application/x-www-form-urlencoded; charset=UTF-8',
          'sec-ch-ua': '"Not?A_Brand";v="99", "Opera GX";v="97", "Chromium";v="111"',
          'sec-ch-ua-mobile': '?0',
          'sec-ch-ua-platform': '"Windows"',
        },
      }
    )
  })

  // Automatically added sleep
  sleep(1)
}
