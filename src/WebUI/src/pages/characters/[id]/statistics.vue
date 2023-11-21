<script setup lang="ts">
import { use, registerTheme, type ComposeOption } from 'echarts/core';
import { BarChart, type BarSeriesOption } from 'echarts/charts';
import {
  ToolboxComponent,
  type ToolboxComponentOption,
  GridComponent,
  type GridComponentOption,
  TooltipComponent,
  type TooltipComponentOption,
} from 'echarts/components';
import { SVGRenderer } from 'echarts/renderers';
import VChart, { THEME_KEY } from 'vue-echarts';
import theme from '@/theme.json';
import { d } from '@/services/translate-service';

use([ToolboxComponent, BarChart, TooltipComponent, GridComponent, SVGRenderer]);
registerTheme('ovilia-green', theme);
type EChartsOption = ComposeOption<
  // | TitleComponentOption
  // | LegendComponentOption
  ToolboxComponentOption | TooltipComponentOption | GridComponentOption | BarSeriesOption
>;
import {
  eachHourOfInterval,
  eachDayOfInterval,
  subMinutes,
  subHours,
  subDays,
  eachMinuteOfInterval,
} from 'date-fns';

definePage({
  props: true,
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
});

const loading = shallowRef(false);
const loadingOptions = {
  text: 'Loading…',
  color: '#4ea397',
  maskColor: 'rgba(255, 255, 255, 0.4)',
};
const option = shallowRef<EChartsOption>({
  xAxis: {
    type: 'time',
    min: subHours(Date.now(), 1).getTime(),
    max: Date.now(),
    splitLine: {
      show: false,
    },
    splitArea: {
      show: false,
    },
  },
  yAxis: {
    type: 'value',
    splitArea: {
      show: false,
    },
  },
  toolbox: {
    // show: true,
    feature: {
      dataView: { show: true, readOnly: false },
      saveAsImage: { show: true },
    },
  },
  tooltip: {
    trigger: 'axis', // TODO:
    axisPointer: {
      type: 'shadow',
      label: {
        formatter: param => d(new Date(param.value), 'long'),
      },
    },
    // formatter: param => {
    //   const [date, value] = param.data;
    //   return `${date.toISOString()} ${value}`;
    // },
  },
  grid: {
    // left: '3%',
    // right: '4%',
    // bottom: '3%',
    // containLabel: true,
  },
  series: [
    {
      name: 'Battle',
      type: 'bar',
      data: [
        ...eachMinuteOfInterval({
          start: subMinutes(Date.now(), 130),
          end: subMinutes(Date.now(), 120),
        }).map(d => [d, getRandom(70000, 120000)]),
        ...eachMinuteOfInterval({
          start: subMinutes(Date.now(), 20),
          end: subMinutes(Date.now(), 10),
        }).map(d => [d, getRandom(50000, 100000)]),
      ],
    },
  ],
});

const chart = shallowRef<InstanceType<typeof VChart> | null>(null);

function getRandom(min: number, max: number) {
  const floatRandom = Math.random();
  const difference = max - min;
  const random = Math.round(difference * floatRandom);
  const randomWithinRange = random + min;
  return randomWithinRange;
}

enum Zoom {
  '30m' = '30m',
  '1h' = '1h',
  '3h' = '3h',
  '12h' = '12h',
  '2d' = '2d',
  '7d' = '7d',
  '14d' = '14d',
}

// const chart = ref<VueApexChartsComponent | null>(null);
const zoomModel = ref<Zoom>(Zoom['1h']);

const getStart = (zoom: Zoom) => {
  switch (zoom) {
    case Zoom['30m']:
      return subMinutes(Date.now(), 30).getTime();
    case Zoom['1h']:
      return subHours(Date.now(), 1).getTime();
    case Zoom['3h']:
      return subHours(Date.now(), 3).getTime();
    case Zoom['12h']:
      return subHours(Date.now(), 12).getTime();
    case Zoom['2d']:
      return subDays(Date.now(), 2).getTime();
    case Zoom['7d']:
      return subDays(Date.now(), 7).getTime();
    case Zoom['14d']:
      return subDays(Date.now(), 14).getTime();
  }
};

const start = computed(() => getStart(zoomModel.value));
// const total = computed(() => getTotal(zoomModel.value));

const setZoom = () => {
  // chart.value?.dispatchAction({
  //   type:'dataZoom',
  //   start: number,
  //   // percentage of ending position; 0 - 100
  //   end: number,
  // })

  option.value = {
    ...option.value,
    xAxis: {
      ...option.value.xAxis,
      min: start.value,
      max: new Date().getTime(),
    },
  };
};

watch(zoomModel, () => {
  setZoom();
});
</script>

<template>
  <div>
    <div class="mx-auto max-w-2xl">
      <pre>{{ { zoomModel, start } }}</pre>
      <OTabs v-model="zoomModel" type="fill-rounded">
        <OTabItem :value="Zoom['30m']">
          <template #header>30m</template>
        </OTabItem>
        <OTabItem :value="Zoom['1h']">
          <template #header>1h</template>
        </OTabItem>
        <OTabItem :value="Zoom['3h']">
          <template #header>3h</template>
        </OTabItem>
        <OTabItem :value="Zoom['12h']">
          <template #header>12h</template>
        </OTabItem>
        <OTabItem :value="Zoom['2d']">
          <template #header>2d</template>
        </OTabItem>
        <OTabItem :value="Zoom['7d']">
          <template #header>7d</template>
        </OTabItem>
        <OTabItem :value="Zoom['14d']">
          <template #header>14d</template>
        </OTabItem>
      </OTabs>

      <VChart
        class="h-[500px]"
        ref="chart"
        theme="ovilia-green"
        :option="option"
        autoresize
        :loading="loading"
        :loadingOptions="loadingOptions"
      />
    </div>
  </div>
</template>
