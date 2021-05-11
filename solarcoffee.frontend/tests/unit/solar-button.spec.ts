import { shallowMount } from "@vue/test-utils";
import SolarButton from "@/components/SolarButton.vue";

describe("SolarButton.vue", () => {
    it("Displays text in default slot position", () => {
        const wrapper = shallowMount(SolarButton, {
            propsData: {},
            slots: {
                default: "Click Here!"
            }
        })
        expect(wrapper.find("button").text()).toBe("Click Here!");
    });

    it("has underlying button disabled when 'disabled' passed as prop", () => {
        const wrapper = shallowMount(SolarButton, {
            propsData: {
                disabled: true
            },
            slots: {
                default: "I am disabled"
            }
        })
        expect(wrapper.find("input:disabled"));
    });
});
